## Purpose

Stores every outgoing event durably in the application database instead of sending it to the event bus, so that an event exists if and only if the work that raised it was committed. A separate relay (not part of this capability yet) delivers stored events.

## ADDED Requirements

### Requirement: Events are stored, not sent
The system SHALL store each event that is raised through the event publisher as one record in the outbox table of the application database. The event publisher SHALL NOT send events to the event bus.

#### Scenario: Operation raises one event
- **WHEN** an operation raises an `AuthorAddedEvent` and completes its unit of work
- **THEN** the outbox table contains exactly one new record for that event
- **AND** no request is made to the event bus

#### Scenario: Operation raises several events
- **WHEN** an operation raises three events in one call and completes its unit of work
- **THEN** the outbox table contains three new records, one for each event

### Requirement: Outbox record content
Each outbox record SHALL hold the EventId, the event source, the detail type, the full event detail (body and tags), the type name of the command or event that caused it, and the time it was stored. Each outbox record SHALL be marked as not yet published. The detail type and tags SHALL be the same values that the publisher produced before this change.

#### Scenario: Record for a domain event
- **WHEN** an `AuthorAddedEvent` raised while handling an `AddAuthorCommand` is stored
- **THEN** the record has detail type `AuthorAddedEvent` and source message type `AddAuthorCommand`
- **AND** the stored detail contains the event body and the tags `general`, `author`, `added`, `success`, `event`
- **AND** the record has no published time

#### Scenario: Record for a failure event
- **WHEN** an `AddAuthorCommand` fails validation
- **THEN** the stored record has detail type `AddAuthorValidationFailedEvent`
- **AND** the stored tags include `validation-failed`

### Requirement: EventId in the event body
The system SHALL generate a new, unique EventId for each stored event and SHALL write it into the event detail as the `event_id` property of `body`. The EventId in the body SHALL equal the key of the outbox record. This SHALL apply to every event type, including failure events, without any change to the event's own definition.

#### Scenario: Event type already defines an event id
- **WHEN** an event whose own definition has a property that serializes to `event_id` is raised
- **THEN** the event is not stored
- **AND** an error is raised that names the event type

#### Scenario: Body carries the EventId
- **WHEN** an `AuthorAddedEvent` is stored
- **THEN** the stored detail has a `body.event_id` property that is a non-empty GUID
- **AND** that value equals the outbox record's key
- **AND** the values of the event's own properties are unchanged

#### Scenario: Each event gets its own EventId
- **WHEN** two events are raised in one call
- **THEN** the two stored records have different EventIds

### Requirement: Events commit with the business change
Events raised by an operation SHALL be committed in the same database transaction as the business data that the operation changes. If that transaction does not commit, the outbox SHALL contain no record for those events.

#### Scenario: Business change and event commit together
- **WHEN** `AddAuthorCommand` is handled successfully
- **THEN** the new author and the `AuthorAddedEvent` outbox record are both present after the single commit

#### Scenario: Commit fails
- **WHEN** an operation raises an event and its unit of work fails to commit
- **THEN** the outbox contains no record for that event
- **AND** the business change is not present

#### Scenario: Event raised but unit of work never completed
- **WHEN** an operation raises an event and returns without completing its unit of work
- **THEN** the outbox contains no record for that event

### Requirement: Failure events are stored independently
Authorization-failed, validation-failed and unhandled-exception events SHALL be stored in the outbox in a transaction of their own, independent of any uncommitted work of the failed message.

#### Scenario: Validation failure
- **WHEN** a command fails validation
- **THEN** one validation-failed record is committed to the outbox
- **AND** no business data is changed

#### Scenario: Operation throws after staging work
- **WHEN** an operation stages business changes and an event, and then throws an exception
- **THEN** one unhandled-exception record is committed to the outbox
- **AND** neither the staged business changes nor the staged event are committed

### Requirement: Event detail uses lower snake_case
Every property name in a stored event detail SHALL be lower snake_case. This SHALL apply to the properties of the event body at every nesting level, whatever the casing of the event's own definition. Tag values are not property names and are unchanged.

#### Scenario: Domain event body
- **WHEN** an `AuthorAddedEvent` with author id 42, first name `Dr.` and last name `Seuss` is stored
- **THEN** the stored body has exactly the properties `author_id`, `first_name`, `last_name` and `event_id`
- **AND** the stored body has no `AuthorId`, `authorId`, `FirstName` or `firstName` property

#### Scenario: Failure event body
- **WHEN** an unhandled-exception event is stored
- **THEN** the stored body has the properties `reason` and `event_id`
