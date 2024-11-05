using GenericHandlers.Domain.Models;

namespace GenericHandlers.Persistence.Repositories;

public interface IBookRepository
{
    Task<Book?> GetAsync(int id);

    Task AddAsync(Book book);
}