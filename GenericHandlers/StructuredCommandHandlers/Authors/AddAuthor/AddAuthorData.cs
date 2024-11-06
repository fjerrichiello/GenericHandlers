using GenericHandlers.Domain.Models;

namespace GenericHandlers.StructuredCommandHandlers.Authors.AddAuthor;

public record AddAuthorData(Author? Author, string FirstName, string LastName);