using GenericHandlers.Domain.Models;

namespace GenericHandlers.Persistence.Repositories;

public interface IAuthorRepository
{
    Task<Author?> GetAsync(int id);

    Task<Author?> GetAsync(string firstName, string lastName);

    Task AddAsync(Author author);
}