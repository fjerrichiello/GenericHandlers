namespace GenericHandlers.Structured.Persistence.UnitOfWork;

public interface IUnitOfWork
{
    Task CompleteAsync();
}