using CommonWithEventFactories.Events.DividedCommand;
using CommonWithEventFactories.Messaging;
using CommonWithEventFactories.Operations;

namespace GenericHandlersWithEventFactories.EventHandlers.DividedEventHandler;

public class DividedEventOperation : IEventPublishingOperation<DividedEvent, EventMetadata, DividedEventVerifiedData>
{
    public async Task ExecuteAsync(MessageContainer<DividedEvent, EventMetadata> container,
        DividedEventVerifiedData data, IEventPublisher eventPublisher)
    {
        await Task.Delay(250);

        //Do Work
        //
        await eventPublisher.PublishAsync(container, new DividedValueChangedEvent(data.Value1));
    }
}