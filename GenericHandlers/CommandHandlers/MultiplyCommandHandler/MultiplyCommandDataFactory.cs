using Common.DataFactory;
using Common.Messaging;
using GenericHandlers.Commands;

namespace GenericHandlers.CommandHandlers.MultiplyCommandHandler;

public class
    MultiplyCommandDataFactory : IDataFactory<MultiplyCommand, CommandMetadata, MultiplyCommandUnverifiedData,
    MultiplyCommandVerifiedData>
{
    public async Task<MultiplyCommandUnverifiedData> GetDataAsync(
        MessageContainer<MultiplyCommand, CommandMetadata> container)
    {
        await Task.Delay(250);
        return new MultiplyCommandUnverifiedData(Random.Shared.Next(100));
    }

    public MultiplyCommandVerifiedData GetVerifiedData(MultiplyCommandUnverifiedData unverifiedData)
    {
        ArgumentNullException.ThrowIfNull(unverifiedData.Value1);

        return new MultiplyCommandVerifiedData(unverifiedData.Value1.Value);
    }
}