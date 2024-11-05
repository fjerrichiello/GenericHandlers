using GenericHandlers.Domain.Models;

namespace GenericHandlers.Persistence.Repositories;

public interface IAuthorRepository
{
    Task<Author?> GetAsync(int id);

    Task AddAsync(Author author);
}