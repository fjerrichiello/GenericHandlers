using System.ComponentModel.DataAnnotations;

namespace GenericHandlers.Persistence.Models;

public class Author
{
    public Author(Domain.Models.Author author) : this(author.Id, author.FirstName, author.LastName)
    {
    }

    public Author(int id, string firstName, string lastName)
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
    }

    [Key]
    public int Id { get; init; }

    public string FirstName { get; init; }

    public string LastName { get; init; }

    public ICollection<BookEntity> Books { get; set; } = [];
}