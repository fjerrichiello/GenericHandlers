using GenericHandlers.Domain.Models;

namespace GenericHandlers.Persistence.Repositories;

public class AuthorRepository(ApplicationDbContext _context) : IAuthorRepository
{
    public async Task<Author?> GetAsync(int id)
    {
        var entity = await _context.Authors.FindAsync(id);

        return entity is null ? null : new Author(entity);
    }


    public async Task AddAsync(Author author)
        => await _context.Authors.AddAsync(new(author));
}