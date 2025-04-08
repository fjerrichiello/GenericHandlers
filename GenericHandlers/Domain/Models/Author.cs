using Common.Models.Authors;

namespace GenericHandlers.Domain.Models;

public record Author(int Id, string FirstName, string LastName) : IAuthorLike
{
    public Author(AuthorEntity entity) : this(entity.Id, entity.FirstName, entity.LastName)
    {
    }

    public static Author? Null => null;

    public Author? ToAuthor(AuthorEntity? authorEntity)
    {
        return authorEntity is null ? null : new Author(Id, FirstName, LastName);
    }
};