using Common.Models.Authors;
using GenericHandlers.Domain.Models;

namespace GenericHandlers.CommandHandlers.Authors.AddAuthor;

public record AddAuthorUnverifiedData(Author? Author, string? FirstName, string? LastName) : IAuthorCommandData<Author>;