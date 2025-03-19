namespace Common.Models.Authors;

public interface IAuthorCommandData<TAuthor>
    where TAuthor : IAuthorLike
{
    TAuthor? Author { get; }
}