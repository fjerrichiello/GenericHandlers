namespace GenericHandlers.Structured.Domain.Models;

public record EditBookRequest(int AuthorId, string Title, string NewTitle)
{
    public EditBookRequest(BookRequestEntity bookRequest) : this(bookRequest.AuthorId, bookRequest.Title,
        bookRequest.NewTitle)
    {
    }
};