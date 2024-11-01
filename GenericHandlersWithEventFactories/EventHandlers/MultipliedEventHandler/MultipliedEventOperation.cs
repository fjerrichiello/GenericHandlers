using CommonWithEventFactories.Events.MultipliedCommand;
using CommonWithEventFactories.Messaging;
using CommonWithEventFactories.Operations;

namespace GenericHandlersWithEventFactories.EventHandlers.MultipliedEventHandler;

public class MultipliedEventOperation : IEventOperation<MultipliedEvent, EventMetadata, MultipliedEventVerifiedData>
{
    public async Task ExecuteAsync(MessageContainer<MultipliedEvent, EventMetadata> container,
        MultipliedEventVerifiedData data)
    {
        await Task.Delay(250);

        //Do Work
        Console.WriteLine("Operated");
    }
}