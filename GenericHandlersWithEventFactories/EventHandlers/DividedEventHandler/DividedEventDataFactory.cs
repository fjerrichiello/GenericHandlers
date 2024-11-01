using CommonWithEventFactories.DataFactory;
using CommonWithEventFactories.Events.DividedCommand;
using CommonWithEventFactories.Messaging;

namespace GenericHandlersWithEventFactories.EventHandlers.DividedEventHandler;

public class
    DividedEventDataFactory : IDataFactory<DividedEvent, EventMetadata, DividedEventUnverifiedData,
    DividedEventVerifiedData>
{
    public async Task<DividedEventUnverifiedData> GetDataAsync(
        MessageContainer<DividedEvent, EventMetadata> container)
    {
        await Task.Delay(250);
        return new DividedEventUnverifiedData(Random.Shared.Next(100));
    }

    public DividedEventVerifiedData GetVerifiedData(DividedEventUnverifiedData unverifiedData)
    {
        ArgumentNullException.ThrowIfNull(unverifiedData.Value1);

        return new DividedEventVerifiedData(unverifiedData.Value1.Value);
    }
}