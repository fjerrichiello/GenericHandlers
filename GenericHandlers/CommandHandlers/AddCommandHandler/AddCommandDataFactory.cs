using Common.DataFactory;
using Common.Messaging;
using GenericHandlers.Commands;

namespace GenericHandlers.CommandHandlers.AddCommandHandler;

public class
    AddCommandDataFactory : IDataFactory<AddCommand, CommandMetadata, AddCommandUnverifiedData, AddCommandVerifiedData>
{
    public async Task<AddCommandUnverifiedData> GetDataAsync(MessageContainer<AddCommand, CommandMetadata> container)
    {
        await Task.Delay(250);
        return new AddCommandUnverifiedData(Random.Shared.Next(100));
    }

    public AddCommandVerifiedData GetVerifiedData(AddCommandUnverifiedData unverifiedData)
    {
        ArgumentNullException.ThrowIfNull(unverifiedData.Value1);

        return new AddCommandVerifiedData(unverifiedData.Value1.Value);
    }
}