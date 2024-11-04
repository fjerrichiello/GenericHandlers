using Common.Events.MultiplyCommand;
using Common.Messaging;
using Common.Operations;

namespace GenericHandlers.EventHandlers.MultipliedEventHandler;

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