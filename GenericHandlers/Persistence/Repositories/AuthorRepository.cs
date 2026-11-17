using GenericHandlers.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace GenericHandlers.Persistence.Repositories;

public class AuthorRepository(ApplicationDbContext _context) : IAuthorRepository
{
    public async Task<Author?> GetAsync(int id)
    {
        var entity = await _context.Authors.FindAsync(id);

        return entity is null ? null : new Author(entity);
    }

    public async Task<Author?> GetAsync(string firstName, string lastName)
    {
        var entity = await _context.Authors.FirstOrDefaultAsync(x =>
            x.FirstName == firstName && x.LastName == lastName);

        return entity is null ? null : new Author(entity);
    }


    public async Task AddAsync(Author author)
        => await _context.Authors.AddAsync(new(author));
}