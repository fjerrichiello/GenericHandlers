using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Common.Enums;
using GenericHandlers.Domain.Models;

namespace GenericHandlers.Persistence.Models;

public class BookRequest
{
    public BookRequest(AddBookRequest addBookRequest)
    {
        Id = Random.Shared.Next(1000000);
        AuthorId = addBookRequest.AuthorId;
        Title = addBookRequest.Title;
        NewTitle = addBookRequest.Title;
        RequestType = RequestType.Add;
        ApprovalStatus = ApprovalStatus.Pending;
    }

    public BookRequest(EditBookRequest editBookRequest)
    {
        Id = Random.Shared.Next(1000000);
        AuthorId = editBookRequest.AuthorId;
        Title = editBookRequest.Title;
        NewTitle = editBookRequest.Title;
        RequestType = RequestType.Edit;
        ApprovalStatus = ApprovalStatus.Pending;
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