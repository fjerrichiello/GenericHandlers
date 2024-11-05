using Common.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Transactions;

public class Transaction<T> : ITransaction<T>
    where T : notnull
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public Transaction(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<Result> ExecuteAsync<TMessage, TMetadata>(
        Func<T, Task<Result>> operation,
        MessageContainer<TMessage, TMetadata> container)
        where TMessage : Message
        where TMetadata : MessageMetadata
    {
        return await ExecuteAsync(operation, container, authenticatedUserOverride: null);
    }

    public async Task<Result> ExecuteAsync<TMessage, TMetadata>(
        Func<T, Task<Result>> operation,
        MessageContainer<TMessage, TMetadata> container,
        string? authenticatedUserOverride = null)
        where TMessage : Message
        where TMetadata : MessageMetadata
    {
        await using var scope = _serviceScopeFactory.CreateAsyncScope();

        var metadataAccessor = scope.ServiceProvider
            .GetRequiredService<IMessageMetadataAccessor>();

        metadataAccessor.MessageMetadata = container.Metadata;

        if (!string.IsNullOrWhiteSpace(authenticatedUserOverride))
        {
            metadataAccessor.MessageMetadata = container.Metadata with
            {
                AuthenticatedUser = authenticatedUserOverride
            };
        }

        var service = scope.ServiceProvider.GetRequiredService<T>();

        return await operation(service);
    }
}