namespace Common.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<TCommand, TEvent>(
        MessageContainer<TCommand, CommandMetadata> commandContainer,
        IEnumerable<TEvent> events)
        where TCommand : Message
        where TEvent : Message;

    Task PublishAsync<TSourceEvent, TEvent>(
        MessageContainer<TSourceEvent, EventMetadata> eventContainer,
        IEnumerable<TEvent> eventBodies)
        where TSourceEvent : Message
        where TEvent : Message;
    
}