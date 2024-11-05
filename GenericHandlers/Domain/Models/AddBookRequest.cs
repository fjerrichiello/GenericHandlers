namespace GenericHandlers.Domain.Models;

public record AddBookRequest(int AuthorId, string Title)
{
    public AddBookRequest(BookRequestEntity bookRequest) : this(bookRequest.AuthorId, bookRequest.Title)
    {
    }
};