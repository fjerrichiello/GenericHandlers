using System.ComponentModel.DataAnnotations;
using Common.Enums;
using GenericHandlers.Domain.Models;

namespace GenericHandlers.Persistence.Models;

public class BookRequest
{
    public BookRequest(AddBookRequest addBookRequest) : this(addBookRequest.AuthorId, addBookRequest.Title,
        addBookRequest.Title, RequestType.Add, ApprovalStatus.Pending)
    {
    }

    public BookRequest(EditBookRequest editBookRequest) : this(editBookRequest.AuthorId, editBookRequest.Title,
        editBookRequest.NewTitle, RequestType.Edit, ApprovalStatus.Pending)
    {
    }

    public BookRequest(int authorId, string title, string newTitle, RequestType requestType,
        ApprovalStatus approvalStatus)
    {
        Id = Random.Shared.Next(1000000);
        AuthorId = authorId;
        Title = title;
        NewTitle = newTitle;
        RequestType = requestType;
        ApprovalStatus = approvalStatus;
    }

    [Key]
    public int Id { get; init; }

    public int AuthorId { get; init; }

    public string Title { get; init; }

    public string NewTitle { get; init; }

    public RequestType RequestType { get; init; }

    public ApprovalStatus ApprovalStatus { get; init; }

    [Timestamp]
    public uint? Version { get; set; }
}