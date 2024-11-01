using CommonWithEventFactories.DataFactory;
using CommonWithEventFactories.Events.MultipliedCommand;
using CommonWithEventFactories.Messaging;

namespace GenericHandlersWithEventFactories.EventHandlers.MultipliedEventHandler;

public class
    MultipliedEventDataFactory : IDataFactory<MultipliedEvent, EventMetadata, MultipliedEventUnverifiedData,
    MultipliedEventVerifiedData>
{
    public async Task<MultipliedEventUnverifiedData> GetDataAsync(
        MessageContainer<MultipliedEvent, EventMetadata> container)
    {
        await Task.Delay(250);
        return new MultipliedEventUnverifiedData(Random.Shared.Next(100));
    }

    public MultipliedEventVerifiedData GetVerifiedData(MultipliedEventUnverifiedData unverifiedData)
    {
        ArgumentNullException.ThrowIfNull(unverifiedData.Value1);

        return new MultipliedEventVerifiedData(unverifiedData.Value1.Value);
    }
}