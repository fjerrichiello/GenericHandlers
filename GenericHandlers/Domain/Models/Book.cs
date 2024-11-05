namespace GenericHandlers.Domain.Models;

public record Book(int Id, int AuthorId, string Title)
{
    public Book(BookEntity book) : this(book.Id, book.AuthorId, book.Title)
    {
    }
};