using GenericHandlers.Structured.Domain.Models;

namespace GenericHandlers.Structured.Persistence.Repositories;

public interface IBookRequestRepository
{
    Task AddAddBookRequestAsync(AddBookRequest addBookRequest);

    Task AddEditBookRequestAsync(EditBookRequest editBookRequest);

    Task<List<AddBookRequest>> GetAddRequests(int authorId, string title);

    Task<List<EditBookRequest>> GetEditRequests(int authorId, string title);
}