using Dumpify;

namespace CommonWithEventFactories.Messaging;

public class EventPublisher : IEventPublisher
{
    public async Task PublishAsync<TCommand, TEvent>(MessageContainer<TCommand, CommandMetadata> commandContainer,
        IEnumerable<TEvent> events) where TCommand : Message where TEvent : Message
    {
        await Task.Delay(250);
        events.ToList().Dump();
    }

    public async Task PublishAsync<TSourceEvent, TEvent>(MessageContainer<TSourceEvent, EventMetadata> eventContainer,
        IEnumerable<TEvent> eventBodies) where TSourceEvent : Message where TEvent : Message
    {
        await Task.Delay(250);
        eventBodies.ToList().Dump();
    }
}