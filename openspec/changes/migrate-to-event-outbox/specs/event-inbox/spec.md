## Purpose

Makes event consumption idempotent. Each consumed event is identified by the EventId in its body, and the system applies the effects of a given EventId at most once, however many times the event is delivered.

## ADDED Requirements

### Requirement: EventId is read from the event body
The system SHALL read the EventId of an incoming event from the `event_id` property of the event detail's `body` and SHALL make it available to the event's handler as part of the event's metadata.

#### Scenario: Event with an EventId
- **WHEN** an event arrives whose detail is `{"body": {"event_id": "3f2a…", "value1": 3}}`
- **THEN** the handler receives metadata whose EventId is `3f2a…`
- **AND** the message still receives `value1` = 3

### Requirement: EventId is mandatory for events
The system SHALL reject an incoming event that has no `body.event_id`, or whose `body.event_id` is not a valid non-empty GUID. A rejected event SHALL NOT be validated or executed, SHALL NOT be recorded in the inbox, and SHALL cause an error to be logged that names the event type. Commands are not affected by this requirement.

#### Scenario: Missing EventId
- **WHEN** an event arrives with no `event_id` in its body
- **THEN** the event's operation is not executed
- **AND** an error is logged
- **AND** the inbox is unchanged

#### Scenario: Malformed EventId
- **WHEN** an event arrives with `body.event_id` = `"not-a-guid"`
- **THEN** the event's operation is not executed
- **AND** an error is logged

#### Scenario: Command without an EventId
- **WHEN** an `AddAuthorCommand` arrives with no `event_id` in its body
- **THEN** the command is handled as before

### Requirement: An EventId is processed at most once
The system SHALL keep an inbox record for each event it has processed, keyed uniquely by EventId, in the application database. When an event arrives whose EventId is already in the inbox, the system SHALL skip it: the event's operation SHALL NOT be executed and no data SHALL change.

#### Scenario: First delivery
- **WHEN** an event with a new EventId arrives and its operation completes successfully
- **THEN** the operation's changes are committed
- **AND** the inbox contains one record with that EventId, the event type and the time received

#### Scenario: Redelivery
- **WHEN** an event arrives whose EventId is already in the inbox
- **THEN** the event's operation is not executed
- **AND** the inbox still contains exactly one record with that EventId

#### Scenario: Same payload, different EventId
- **WHEN** two events with identical body content but different EventIds arrive
- **THEN** both are processed

### Requirement: Inbox record commits with the business change
The inbox record for an event SHALL be committed in the same database transaction as the changes made by the event's operation, including any events that the operation stores in the outbox. If that transaction does not commit, the inbox SHALL NOT contain the EventId, so that a later redelivery is processed.

#### Scenario: Operation fails
- **WHEN** an event's operation throws before its unit of work commits
- **THEN** the inbox does not contain the EventId
- **AND** a redelivery of the same event is processed

#### Scenario: Event fails validation
- **WHEN** an event with a new EventId fails validation
- **THEN** the operation is not executed
- **AND** the inbox does not contain the EventId

#### Scenario: Concurrent duplicate deliveries
- **WHEN** two deliveries of the same EventId are handled at the same time
- **THEN** the changes of at most one of them are committed
- **AND** the inbox contains exactly one record with that EventId

### Requirement: Incoming message properties are lower snake_case
The system SHALL read the properties of an incoming message, for both commands and events, by their lower snake_case names. This SHALL apply to the content of `body`, to path and query-string parameters, and to the metadata properties of the detail (`tags`, `authenticated_user`). A property supplied only under a camelCase or PascalCase name SHALL be treated as absent.

#### Scenario: Command in snake_case
- **WHEN** an `AddAuthorCommand` arrives with body `{"first_name": "Dr.", "last_name": "Seuss"}`
- **THEN** the command is handled with first name `Dr.` and last name `Seuss`

#### Scenario: Command in camelCase
- **WHEN** an `AddAuthorCommand` arrives with body `{"firstName": "Dr.", "lastName": "Seuss"}`
- **THEN** the first name and last name are treated as absent
- **AND** the command fails validation

#### Scenario: Stored event can be consumed
- **WHEN** an event detail exactly as stored in the outbox is delivered to a handler for that event type
- **THEN** every property of the event is populated
- **AND** the handler's metadata EventId equals the outbox record's key
