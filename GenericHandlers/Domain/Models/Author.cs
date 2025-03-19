using Common.Models.Authors;

namespace GenericHandlers.Domain.Models;

public record Author(int Id, string FirstName, string LastName) : IAuthorLike
{
    public Author(AuthorEntity entity) : this(entity.Id, entity.FirstName, entity.LastName)
    {
    }
};