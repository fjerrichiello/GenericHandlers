using Common.Messaging;

namespace Common.Transactions;

public interface ITransaction<out T>
{
    Task<Result> ExecuteAsync<TMessage, TMetadata>(
        Func<T, Task<Result>> operation,
        MessageContainer<TMessage, TMetadata> container)
        where TMessage : Message
        where TMetadata : MessageMetadata;

    Task<Result> ExecuteAsync<TMessage, TMetadata>(
        Func<T, Task<Result>> operation,
        MessageContainer<TMessage, TMetadata> container,
        string? authenticatedUserOverride = null)
        where TMessage : Message
        where TMetadata : MessageMetadata;
}