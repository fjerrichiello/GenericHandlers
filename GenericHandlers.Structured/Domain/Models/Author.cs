namespace GenericHandlers.Structured.Domain.Models;

public record Author(int Id, string FirstName, string LastName)
{
    public Author(AuthorEntity entity) : this(entity.Id, entity.FirstName, entity.LastName)
    {
    }
};