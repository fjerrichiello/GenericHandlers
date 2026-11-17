## 1. Outbox and inbox persistence model (Common)

- [ ] 1.1 Add `OutboxEvent` and `InboxEvent` entities under `Common/Messaging/Outbox/` with the properties in design.md decision 1; verify `dotnet build Common` succeeds
- [ ] 1.2 Add `IOutboxContext` (the two `DbSet`s) and a `ModelBuilder` extension that configures keys, the `jsonb` detail column, and the explicit lower snake_case table, column and primary-key names listed in design.md decision 1; verify `dotnet build Common` succeeds
- [ ] 1.3 Add `IOutboxRepository` (`Add`) and `IInboxRepository` (`ExistsAsync`, `Add`) with generic implementations constrained to `DbContext, IOutboxContext`; verify `dotnet build Common` succeeds
- [ ] 1.4 Add `IOutboxUnitOfWork` and `OutboxRecorder` (adds outbox events, then completes the unit of work), modeled on `PublishedEventRecorder`; verify with a unit test that `RecordAsync` adds every event and calls `CompleteAsync` once
- [ ] 1.5 Add `Registration.AddOutbox<TContext, TUnitOfWork>()` registering the two repositories, the unit of work and the recorder as scoped; verify `dotnet build Common` succeeds

## 2. Application database (GenericHandlers)

- [ ] 2.1 Make `ApplicationDbContext` implement `IOutboxContext` and call the model-builder extension in `OnModelCreating`; verify `dotnet build GenericHandlers` succeeds
- [ ] 2.2 Add `OutboxUnitOfWork : UnitOfWork<ApplicationDbContext>, IOutboxUnitOfWork` under `GenericHandlers/Persistence/UnitOfWork/` and call `services.AddOutbox<ApplicationDbContext, OutboxUnitOfWork>()` in `Program.cs`; verify the app starts (`dotnet run --project GenericHandlers`) without a DI resolution error
- [ ] 2.3 Create the migration with `dotnet ef migrations add AddOutboxAndInbox --project GenericHandlers --context ApplicationDbContext --output-dir Persistence/Migrations`; verify the generated `Up` creates only `outbox_events` and `inbox_events`, that every column and constraint name in it is lower snake_case, and that it touches no existing table
- [ ] 2.4 Apply it to the local database (`docker compose up -d`, then `dotnet ef database update --project GenericHandlers --context ApplicationDbContext`); verify `outbox_events` (primary key `id`) and `inbox_events` (primary key `event_id`) exist in the `handlers` database

## 3. Publisher stores instead of sending

- [ ] 3.1 Add one shared `JsonSerializerOptions` with `JsonNamingPolicy.SnakeCaseLower` in `Common/Messaging/`; verify with a unit test that `AuthorAddedEvent` serializes to exactly `author_id`, `first_name`, `last_name`
- [ ] 3.2 In `EventPublisher`, generate an EventId per event, serialize the body with the shared snake_case options, and inject the id as `body.event_id`; throw `InvalidOperationException` if the serialized body already contains `event_id`; verify with unit tests that the stored detail's `body.event_id` equals the outbox row `Id`, that two events get different ids, and that the collision case throws
- [ ] 3.3 Replace `PublishEntriesAsync` for the two `PublishAsync` overloads with staging through `IOutboxRepository` (no save, no `IEventBridgeClient` call); verify with unit tests that N events produce N `Add` calls with the expected `DetailType`, `Source`, `SourceMessageType`, tags and null `PublishedAt`
- [ ] 3.4 Route `PublishAuthorizationFailedAsync`, `PublishValidationFailedAsync` and `PublishUnhandledExceptionAsync` (all overloads) through `ITransaction<OutboxRecorder>`; verify with unit tests that each stores exactly one event with the existing detail-type suffix and failure tag, and does not use the request-scoped `IOutboxRepository`
- [ ] 3.5 Remove the `IEventBridgeClient` and `ITransaction<PublishedEventRecorder>` dependencies from `EventPublisher`, leaving their DI registrations in place; verify `dotnet build` succeeds and a search shows no reference to `IEventBridgeClient` in `EventPublisher.cs`

## 4. Operations raise events before completing

- [ ] 4.1 Reorder `AddAuthorOperation` to call `PublishAsync` before `CompleteAsync`; update `AddAuthorOperationTests` to assert that order (for example with a Moq `MockSequence`) and verify the test passes

## 5. Consumer reads snake_case and uses EventId as the unique key

- [ ] 5.1 Switch `MessageContainerMapper` to the shared snake_case options for both the message and the metadata; verify with unit tests that a body of `first_name` / `last_name` populates `AddAuthorCommand`, that the camelCase form leaves both properties null, and that `authenticated_user` and `tags` populate the metadata
- [ ] 5.2 Add `Guid EventId { get; init; }` to `EventMetadata` and set it in `MessageContainerMapper` from `detail.body.event_id` when the metadata type is `EventMetadata` (leave `Guid.Empty` when missing or unparsable); verify with unit tests for present, missing and malformed values, and that the message still deserializes its own properties
- [ ] 5.3 In `EventHandler`, reject an event whose `EventId` is `Guid.Empty` with an error log that names the message type; verify with a unit test that the data factory, verifier and operation are not called
- [ ] 5.4 In `EventHandler`, skip an event whose EventId already exists in the inbox; verify with a unit test that the operation is not called and no inbox row is added
- [ ] 5.5 In `EventHandler`, add the `InboxEvent` after validation succeeds and before the operation executes; verify with unit tests that a valid new event adds one inbox row before `ExecuteAsync`, and that an event failing validation adds none
- [ ] 5.6 Confirm `CommandHandler` and `AuthorizedCommandHandler` need no code change; verify the existing command tests pass and a snake_case command without `event_id` is still handled
- [ ] 5.7 Add a round-trip unit test: take the detail that `EventPublisher` stores for an event, pass it to `MessageContainerMapper` for the same event type, and verify every property is populated and the metadata `EventId` equals the outbox row `Id`
- [ ] 5.8 In `HttpSender/Program.cs`, serialize the command body with `JsonNamingPolicy.SnakeCaseLower` while keeping camelCase for the `MessageRequest` envelope, and update the sample payload comments at the bottom of `GenericHandlers/Program.cs` to snake_case; verify the logged request shows `detailType`, `detail.body.first_name` and `detail.body.last_name`

## 6. End-to-end verification

- [ ] 6.1 Run `dotnet test` for the solution and verify all tests pass
- [ ] 6.2 With the app running against the local database, run `HttpSender` to send a valid `AddAuthorCommand`; verify the `Authors` row and one `outbox_events` row (`detail_type` = `AuthorAddedEvent`, `published_at` null, `detail->'body'->>'event_id'` equal to `id`, body keys `author_id`, `first_name`, `last_name`) appear together, and that nothing is written to `PublishedEvents` in `handlersmessaging`
- [ ] 6.3 Post an invalid `AddAuthorCommand` (empty `first_name`) to `/generic-command-events`; verify one `outbox_events` row with `detail_type` = `AddAuthorValidationFailedEvent` and no new `Authors` row
- [ ] 6.4 Run `openspec validate migrate-to-event-outbox --strict` and verify it passes
