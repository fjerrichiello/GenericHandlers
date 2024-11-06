using System.ComponentModel.DataAnnotations;

namespace GenericHandlers.Structured.Persistence.Models;

public class Book
{
    public Book(Domain.Models.Book book) : this(book.Id, book.AuthorId, book.Title)
    {
    }

    public Book(int id, int authorId, string title)
    {
        Id = id;
        AuthorId = authorId;
        Title = title;
    }

    [Key]
    public int Id { get; init; }

    public int AuthorId { get; init; }

    public string Title { get; init; }

    public AuthorEntity Author { get; set; }
}