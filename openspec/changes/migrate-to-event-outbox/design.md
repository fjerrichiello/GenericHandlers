## Context

See proposal.md for motivation. The current state that shapes the approach:

- `Common/Messaging/EventPublisher.cs` builds `RequestEntry` objects (source, detail type, detail JSON of `{ body, tags }`), sends them through `IEventBridgeClient`, and then records the accepted ones as `PublishedEvent` rows in the **messaging** database through `ITransaction<PublishedEventRecorder>`.
- The application has two databases: `handlers` (`ApplicationDbContext`, business data) and `handlersmessaging` (`ApplicationMessagingDbContext`). Rows in two databases cannot commit atomically, so the outbox and inbox must live in `handlers`.
- `Common` cannot reference `ApplicationDbContext` or the app's `IUnitOfWork` (both are in the `GenericHandlers` project). The existing pattern for this is: `Common` defines the abstraction (`IMessagingUnitOfWork`, `MessagingRepository<TContext>`), the app supplies the context and unit of work, and `Registration.AddMessagingPersistence<,>` wires them.
- `UnitOfWork<TDbContext>.CompleteAsync` is single-use per scope. `Transaction<T>` runs a delegate in a fresh DI scope, which gives a fresh context and unit of work.
- The HTTP endpoint resolves the orchestrator, handler, operation, repositories and publisher from one request scope, so they share one `ApplicationDbContext` instance.
- `AddAuthorOperation` calls `_unitOfWork.CompleteAsync()` and then `_eventPublisher.PublishAsync(...)`.
- Handlers call the publisher for failure events outside any operation (validation failed, authorization failed, and in the `catch` block for unhandled exceptions).
- `MessageContainerMapper` merges `detail.body` into the message and deserializes `detail` into the metadata type. `EventMetadata` has no members of its own.
- `EventHandler` has no publisher and no dedupe. No event operation exists in the `GenericHandlers` app; only `AddAuthorCommand`.

Decisions confirmed with the user before drafting: outbox rows commit in the same transaction as the business change; the dedupe table is a new table in the main database; an event without an EventId is rejected and logged. After the first draft the user added that the production system uses lower snake_case for everything; decisions 1 and 8 follow from that.

## Goals / Non-Goals

**Goals:**
- Keep the `IEventPublisher` interface and its call sites unchanged in shape, so that the same migration in the production system is a swap of implementation plus a reorder inside operations.
- Make the outbox and inbox reusable from `Common` by any application context, following the existing `AddMessagingPersistence` pattern.
- Keep detail type, source and tag computation exactly as they are today.

**Non-Goals:**
- The relay (polling, batching to `PutEvents`, retries, marking rows published, cleanup of old rows).
- Any change to the messaging database, its context, its migration, or `PublishedEventRecorder`. They stay registered and unused by the publisher.
- Command idempotency.
- Renaming existing tables or columns (`Authors`, `Books`, `BookRequests`, the messaging tables) to snake_case.
- Changing tag values, detail-type names, or the HTTP envelope of `MessageRequest` (`detailType`, `detail`), which stands in for the EventBridge envelope and is not part of the message body.

## Decisions

### 1. Entities and tables live in `Common`, mapped onto the application context

Add to `Common/Messaging/Outbox/`:

- `OutboxEvent`: `Id` (Guid, primary key, equals the EventId), `Source`, `DetailType`, `Detail` (`JsonElement`, `jsonb`), `SourceMessageType`, `CreatedAt` (`DateTimeOffset`), `PublishedAt` (`DateTimeOffset?`, null means not yet published).
- `InboxEvent`: `EventId` (Guid, primary key), `MessageType`, `ReceivedAt` (`DateTimeOffset`).
- `IOutboxContext`: an interface with `DbSet<OutboxEvent> OutboxEvents` and `DbSet<InboxEvent> InboxEvents`.
- A `ModelBuilder` extension that configures both entities: keys, the `jsonb` column, and explicit lower snake_case names for the tables (`outbox_events`, `inbox_events`), every column (`id`, `source`, `detail_type`, `detail`, `source_message_type`, `created_at`, `published_at`; `event_id`, `message_type`, `received_at`) and the primary-key constraints (`pk_outbox_events`, `pk_inbox_events`).

The names are set explicitly with `ToTable` / `HasColumnName` / `HasName`, so no naming-convention package is added and the existing PascalCase tables of this project are not touched. C# property and `DbSet` names stay PascalCase.

*Alternative considered:* `EFCore.NamingConventions` with `UseSnakeCaseNamingConvention()`. Rejected: it applies to the whole context and would rename the existing tables in the next migration.

`ApplicationDbContext` implements `IOutboxContext` and calls the extension in `OnModelCreating`. One new EF migration for `ApplicationDbContext` creates both tables.

*Why an interface and not a base class like `MessagingContext<TContext>`:* `ApplicationDbContext` already derives from `DbContext` and owns business entities; an interface adds the two sets without changing its base type.

*Alternative considered:* put the tables in `ApplicationMessagingDbContext`. Rejected: a different database cannot share the business transaction.

`PublishedAt` is included now, although nothing sets it, so that the relay change does not need a second migration to the same new table. No index for the relay's query is added here; the relay change owns its access path.

### 2. The publisher stages; the operation's unit of work commits

`EventPublisher` keeps its public methods and its private detail-type/tag helpers. `PublishEntriesAsync` is replaced: for each entry it creates an `OutboxEvent` and adds it through `IOutboxRepository` (`OutboxRepository<TContext> where TContext : DbContext, IOutboxContext`), which wraps the request-scoped application context. It does not save. The operation's existing `IUnitOfWork.CompleteAsync()` then commits business rows and outbox rows in one `SaveChangesAsync`, which is one transaction.

`EventPublisher` no longer depends on `IEventBridgeClient` or `ITransaction<PublishedEventRecorder>`. The `Chunk(MaxEntriesPerRequest)` batching is removed; batching is a relay concern.

Consequence: an operation must call `PublishAsync` **before** `CompleteAsync`. `AddAuthorOperation` is reordered. An event raised after `CompleteAsync` would be staged and never saved; because `UnitOfWork` is single-use, a second `CompleteAsync` throws, so this mistake fails loudly in any operation that tries to save again, and silently otherwise (see Risks).

*Alternative considered:* the publisher saves on its own in a fresh scope. Rejected by the user; it keeps the lost-event window that the outbox exists to close.

*Alternative considered:* an EF `SaveChangesInterceptor` that collects events. Rejected: more hidden behavior than a test bed for a production migration needs.

### 3. Failure events use a fresh scope

The three failure paths (`PublishAuthorizationFailedAsync`, `PublishValidationFailedAsync`, `PublishUnhandledExceptionAsync`) have no unit of work that will commit, and on the exception path the request-scoped context can hold half-staged changes that must not be saved. For these, `EventPublisher` runs `ITransaction<OutboxRecorder>`: the recorder gets a fresh `IOutboxRepository` and a fresh `IOutboxUnitOfWork` (defined in `Common`, implemented in the app as a `UnitOfWork<ApplicationDbContext>` subclass, the same way `MessagingUnitOfWork` implements `IMessagingUnitOfWork`), adds the row and completes.

So inside `EventPublisher` there are two private paths: `StageAsync` (domain events) and `StoreNowAsync` (failure events). Which one runs is decided by which public method was called, not by the caller.

*Alternative considered:* a flag or a second interface for callers to choose. Rejected: every current caller's intent is already fixed by the method it calls.

### 4. EventId is generated by the publisher and injected into the body

For each event the publisher generates `Guid.NewGuid()` (the project targets .NET 8; `Guid.CreateVersion7` is not available). It serializes the event body to a `JsonObject` with the shared snake_case options (decision 8), sets `event_id`, and builds the detail as `{ body, tags }`. The same Guid is `OutboxEvent.Id`.

*Why inject and not add `EventId` to the `Message` base record:* `Message` is also the base of commands, and adding a constructor parameter would change every event and command definition and every test that constructs one. Injection makes it "by default" with no change to event types.

The property name is the literal `event_id`. If the serialized body already contains `event_id` (for example because an event type declares its own `EventId` property), the publisher throws `InvalidOperationException` instead of overwriting it.

### 5. The consumer carries EventId in `EventMetadata`

`EventMetadata` gains `Guid EventId { get; init; }`. `MessageContainerMapper` reads `detail.body.event_id`; when the metadata type is `EventMetadata` it sets the value with a `with` expression. A missing or unparsable value leaves `Guid.Empty`. The mapper does not throw for this, so that the rejection and its log line are in one place, the handler. `CommandMetadata` is unchanged.

*Alternative considered:* read the EventBridge envelope `id`. Rejected: `MessageRequest` does not carry it, and the user asked for the body value to be the unique key (it survives re-publishing and replays, the envelope id does not).

### 6. `EventHandler` checks and stages the inbox record

`EventHandler` gains an `IInboxRepository` (`ExistsAsync(Guid)`, `Add(InboxEvent)`), backed by the same request-scoped context. Order inside `HandleAsync`:

1. `EventId == Guid.Empty` → log error, return.
2. `ExistsAsync(EventId)` → log information, return.
3. Data factory and validation, as today. Invalid → return, nothing staged.
4. `Add(new InboxEvent(...))`, then `_operation.ExecuteAsync(...)`. The operation's `CompleteAsync` commits the inbox row, the business change and any outbox rows together.

The primary key on `EventId` is the real guarantee. The `ExistsAsync` check is only the cheap path for the common redelivery case. For two concurrent deliveries, the second commit fails with a unique violation (`23505`, already translated by `UnitOfWork` to `InvalidOperationException`), its whole transaction rolls back, and the existing `catch` logs it.

*Alternative considered:* write the inbox row in its own transaction before the operation. Rejected: a crash after that write marks the event processed although its work never committed.

### 7. Registration

`Registration.AddOutbox<TContext, TUnitOfWork>()` registers `IOutboxRepository`, `IInboxRepository`, `IOutboxUnitOfWork` and `OutboxRecorder`. `Program.cs` calls `services.AddOutbox<ApplicationDbContext, OutboxUnitOfWork>()`. `AddEventHandlersAndNecessaryWork` keeps registering `IEventBridgeClient` → `MockEventBridgeClient` so the relay change can use it; `AddMessagingPersistence` stays as is.

### 8. One lower snake_case JSON policy for writing and reading

The production system uses lower snake_case for everything, so this change makes the message JSON consistent instead of carrying the current mismatch forward (bodies written with default options, PascalCase; read with a camelCase policy; metadata read with default options).

Add one shared `JsonSerializerOptions` instance in `Common/Messaging/` with `PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower` (built into `System.Text.Json` 8, no new package). It is used in three places:

- `EventPublisher`, for the event body. The anonymous envelope `{ body, tags }` already has snake_case-compatible names.
- `MessageContainerMapper`, for the message (replacing the camelCase options created per call) and for the metadata (replacing default options). `AuthenticatedUser` is therefore read from `authenticated_user`, and `Tags` from `tags`.
- `HttpSender/Program.cs` has its own options (it does not reference `Common`); its policy is changed to `SnakeCaseLower`. Its `MessageRequest` envelope then serializes as `detail_type`, which the endpoint would not bind, so the envelope keeps camelCase options and only the command body is serialized with snake_case.

Dictionary keys are not renamed by a property naming policy; the validation-failed event's keys are already snake_case through `ToDictionarySnakeCase`.

*Alternative considered:* keep `event_id` as a literal and leave the rest of the casing alone. Rejected: a stored event could then not be consumed by this project's own mapper, and the test bed would differ from production in the contract it is meant to model.

## Risks / Trade-offs

- [Snake_case reading is a breaking change for every sender] → a command sent in camelCase now binds no properties and fails validation. Mitigation: `HttpSender` is updated in this change; the sample payloads in the comments at the bottom of `Program.cs` are updated with it.
- [`JsonNamingPolicy.SnakeCaseLower` and the project's regex `ToSnakeCase` can disagree on unusual names (consecutive capitals, digits)] → message property names and validation error keys could differ for such a property. No current message has one; a unit test pins `AuthorAddedEvent` and `AddAuthorCommand`.

- [An operation raises an event after `CompleteAsync`, or never completes] → the event is silently not stored. Mitigation: the spec states it; the reordered `AddAuthorOperation` and its test (which asserts call order) are the reference pattern. A guard that detects un-flushed outbox rows at the end of the request is a possible follow-up.
- [An event operation does its work through `ITransaction<T>` (fresh scope) instead of the request-scoped unit of work] → the inbox row staged by `EventHandler` is in a different context and is never committed, so dedupe does not happen. Mitigation: documented here; no such operation exists in this project. This must be checked against the production system before the pattern is ported.
- [An event operation makes no changes and never completes the unit of work] → no inbox record; a redelivery runs the operation again. Acceptable: the operation had no effect to duplicate.
- [No relay yet] → after this change no event leaves the service and `outbox_events` grows without bound. Acceptable for the test bed; the relay is the next change.
- [Strict EventId rejection] → events from any publisher that has not adopted `body.event_id` are dropped with an error log. Chosen deliberately by the user; in production the publishers must be migrated before the consumers.
- [Failure event store itself fails] → the exception escapes the handler's `catch` block, as an `IEventBridgeClient` failure does today. No change in behavior.
- [`MessagingContext` tables are now dead weight] → left in place on purpose to keep this change small; the relay change decides whether `PublishedEvents` is still wanted.

## Migration Plan

1. Apply the new `ApplicationDbContext` migration to `handlers` (`dotnet ef database update --context ApplicationDbContext`). It only adds two tables.
2. Deploy the code. From this point events are stored, not sent.
3. Rollback: redeploy the previous build; the two tables can stay (unused) or be removed with the migration's `Down`.

## Open Questions

- Whether `OutboxEvent` should also record the causing message's identifier (the consumed event's EventId) for tracing. Not needed by any requirement here; it would be a nullable column added later.
