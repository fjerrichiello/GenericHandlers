using System.ComponentModel.DataAnnotations;

namespace GenericHandlers.Persistence.Models;

public class Author
{
    public Author(Domain.Models.Author author)
    {
        Id = author.Id;
        FirstName = author.FirstName;
        LastName = author.LastName;
    }

    [Key]
    public int Id { get; init; }

    public string FirstName { get; init; }

    public string LastName { get; init; }
}