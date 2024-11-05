using System.ComponentModel.DataAnnotations;

namespace GenericHandlers.Persistence.Models;

public class Book
{
    public Book(Domain.Models.Book book)
    {
        Id = book.Id;
        AuthorId = book.AuthorId;
        Title = book.Title;
    }

    [Key]
    public int Id { get; init; }

    public int AuthorId { get; init; }

    public string Title { get; init; }

    public AuthorEntity Author { get; set; }
}