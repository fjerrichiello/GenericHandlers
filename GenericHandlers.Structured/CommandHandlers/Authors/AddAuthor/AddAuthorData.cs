using GenericHandlers.Structured.Domain.Models;

namespace GenericHandlers.Structured.CommandHandlers.Authors.AddAuthor;

public record AddAuthorData(Author? Author, string FirstName, string LastName);