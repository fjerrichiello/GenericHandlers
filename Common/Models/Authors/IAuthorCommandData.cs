namespace Common.Models.Authors;

public interface IAuthorCommandData<out TAuthor>
    where TAuthor : IAuthorLike
{
    TAuthor? Author { get; }
}