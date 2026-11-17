namespace Common.Messaging.Persistence;

/// <summary>
/// Unit of work for the messaging database. Like the application's unit of work it is
/// single-use: one <see cref="CompleteAsync"/> per scope, so each save runs in its own
/// <see cref="Common.Transactions.ITransaction{T}"/>.
/// </summary>
public interface IMessagingUnitOfWork
{
    Task CompleteAsync();
}
