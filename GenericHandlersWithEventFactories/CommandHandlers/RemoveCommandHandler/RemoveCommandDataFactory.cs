using CommonWithEventFactories.DataFactory;
using CommonWithEventFactories.Messaging;
using GenericHandlersWithEventFactories.Commands;

namespace GenericHandlersWithEventFactories.CommandHandlers.RemoveCommandHandler;

public class
    RemoveCommandDataFactory : IDataFactory<RemoveCommand, CommandMetadata, RemoveCommandUnverifiedData,
    RemoveCommandVerifiedData>
{
    public async Task<RemoveCommandUnverifiedData> GetDataAsync(
        MessageContainer<RemoveCommand, CommandMetadata> container)
    {
        await Task.Delay(250);
        return new RemoveCommandUnverifiedData(Random.Shared.Next(100));
    }

    public RemoveCommandVerifiedData GetVerifiedData(RemoveCommandUnverifiedData unverifiedData)
    {
        ArgumentNullException.ThrowIfNull(unverifiedData.Value1);

        return new RemoveCommandVerifiedData(unverifiedData.Value1.Value);
    }
}