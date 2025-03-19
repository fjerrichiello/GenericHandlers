using Common.Models.Authors;
using GenericHandlers.Domain.Models;

namespace GenericHandlers.CommandHandlers.Authors.AddAuthor;

public record AddAuthorUnverifiedData(IAuthorLike? Author, string? FirstName, string? LastName)
    : IAuthorCommandData<IAuthorLike>;