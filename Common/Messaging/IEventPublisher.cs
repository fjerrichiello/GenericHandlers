using Common.Messaging.Publishing;

namespace Common.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<TCommand, TEvent>(
        MessageContainer<TCommand, CommandMetadata> commandContainer,
        IEnumerable<TEvent> eventBodies)
        where TCommand : Message
        where TEvent : Message;

    Task PublishAsync<TSourceEvent, TEvent>(
        MessageContainer<TSourceEvent, EventMetadata> eventContainer,
        IEnumerable<TEvent> eventBodies)
        where TSourceEvent : Message
        where TEvent : Message;


    Task PublishAuthorizationFailedAsync<TCommand>(
        MessageContainer<TCommand, CommandMetadata> commandContainer,
        AuthorizationFailedEvent eventBody)
        where TCommand : Message;

    Task PublishValidationFailedAsync<TCommand>(
        MessageContainer<TCommand, CommandMetadata> commandContainer,
        ValidationFailedEvent eventBody)
        where TCommand : Message;

    Task PublishValidationFailedAsync<TSourceEvent>(
        MessageContainer<TSourceEvent, EventMetadata> eventContainer,
        ValidationFailedEvent eventBody)
        where TSourceEvent : Message;

    Task PublishUnhandledExceptionAsync<TCommand>(
        MessageContainer<TCommand, CommandMetadata> commandContainer,
        UnhandledExceptionEvent eventBody)
        where TCommand : Message;

    Task PublishUnhandledExceptionAsync<TSourceEvent>(
        MessageContainer<TSourceEvent, EventMetadata> eventContainer,
        UnhandledExceptionEvent eventBody)
        where TSourceEvent : Message;
}