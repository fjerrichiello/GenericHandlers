using GenericHandlers.Structured.Domain.Models;

namespace GenericHandlers.Structured.Persistence.Repositories;

public interface IBookRepository
{
    Task<Book?> GetAsync(int id);

    Task AddAsync(Book book);
}