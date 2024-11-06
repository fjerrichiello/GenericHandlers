using GenericHandlers.Structured.Domain.Models;

namespace GenericHandlers.Structured.Persistence.Repositories;

public class BookRepository(ApplicationDbContext _context) : IBookRepository
{
    public async Task<Book?> GetAsync(int id)
    {
        var entity = await _context.Books.FindAsync(id);

        return entity is null ? null : new Book(entity);
    }


    public async Task AddAsync(Book book)
        => await _context.Books.AddAsync(new(book));
}