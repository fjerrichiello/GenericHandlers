namespace GenericHandlers.Persistence.UnitOfWork;

public interface IUnitOfWork
{
    Task CompleteAsync();
}