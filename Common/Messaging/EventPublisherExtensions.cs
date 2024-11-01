namespace Common.Messaging;

public static class EventPublisherExtensions
{
    public static async Task PublishAsync<TCommand, TEvent>(
        this IEventPublisher eventPublisher,
        MessageContainer<TCommand, CommandMetadata> commandContainer,
        TEvent eventBody)
        where TCommand : Message
        where TEvent : Message
    {
        await eventPublisher.PublishAsync(commandContainer, [eventBody]);
    }

    public static async Task PublishAsync<TSourceEvent, TEvent>(
        this IEventPublisher eventPublisher,
        MessageContainer<TSourceEvent, EventMetadata> eventContainer,
        TEvent eventBody)
        where TSourceEvent : Message
        where TEvent : Message
    {
        await eventPublisher.PublishAsync(eventContainer, [eventBody]);
    }
}