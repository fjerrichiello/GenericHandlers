## Why

Today `EventPublisher` sends events to EventBridge directly, after the operation has already committed its business data (`AddAuthorOperation` calls `CompleteAsync` and then `PublishAsync`). A crash or a failed `PutEvents` call between those two steps loses the event, and consumers have no stable identifier with which to detect a redelivered event. The production system this project models has been asked to move to a transactional outbox; this project is the test bed for that migration.

## What Changes

- Add two tables to the main application database (`handlers`, `ApplicationDbContext`): an outbox table for events waiting to be published, and an inbox table that records each consumed event by its EventId.
- **BREAKING** `IEventPublisher` no longer sends anything to EventBridge. It stores each event as an outbox row. For events raised by an operation, the rows are staged on the request's database context and commit in the same transaction as the business change.
- **BREAKING** Operations must raise their events before they complete the unit of work. `AddAuthorOperation` is reordered accordingly.
- Failure events raised by the handlers (authorization failed, validation failed, unhandled exception) are stored in the outbox in their own transaction, because no business transaction commits on those paths.
- Every stored event gets a generated EventId, written into the event detail as `body.event_id`. The same value is the outbox row's key.
- **BREAKING** All message JSON is lower snake_case, to match the production system. The publisher writes event bodies in snake_case (`author_id`, `first_name`), and incoming commands and events are read as snake_case (today they are written in PascalCase and read as camelCase). `HttpSender` is switched to send snake_case.
- The new tables and their columns are named in lower snake_case (`outbox_events`, `inbox_events`). Existing tables are not renamed.
- **BREAKING** Event consumption requires `body.event_id`. An event without a valid one is rejected and logged. An event whose EventId is already in the inbox is skipped. The inbox record commits with the handler's business change.
- Out of scope: the relay that reads the outbox and sends to EventBridge. `IEventBridgeClient` and `MockEventBridgeClient` stay in the codebase, unused by the publisher, for that later change.
- Out of scope: the separate messaging database (`PublishedEvents`, `ReceivedCommands`, `ReceivedEvents`). It is left as it is; the publisher stops writing `PublishedEvents`.
- Out of scope: deduplication of commands. Commands carry no identifier today.

## Capabilities

### New Capabilities
- `event-outbox`: Storing outgoing events in the application database instead of sending them, including the EventId placed in each event body and the transactional guarantees of that storage.
- `event-inbox`: Consuming events idempotently, using the EventId in the event body as the unique key.

### Modified Capabilities

None. `openspec/specs/` is empty.

## Impact

- **Common**: `EventPublisher` (rewritten to store), `EventMetadata` and `MessageContainerMapper` (carry the EventId), `EventHandler` (inbox check), `Registration` (new outbox/inbox registration), new outbox and inbox entities, repositories and recorder under `Common/Messaging/`.
- **GenericHandlers**: `ApplicationDbContext` (two new entity sets), a new EF Core migration under `Persistence/Migrations`, `Program.cs` (registration), `AddAuthorOperation` (publish before complete).
- **UnitTests**: `AddAuthorOperationTests` updated for the new order; new tests for the publisher, the mapper and `EventHandler`.
- **HttpSender**: serializer options switched from camelCase to lower snake_case.
- **Event contract**: every published event body gains an `event_id` property, and body property names change from PascalCase to lower snake_case.
- **Command contract**: command bodies must use lower snake_case property names (`first_name`, not `firstName`).
- **Runtime behavior**: until the relay exists, no event leaves the service. Events accumulate in the outbox table.
- No new package dependencies.
