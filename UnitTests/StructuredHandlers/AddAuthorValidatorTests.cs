using FluentAssertions;
using GenericHandlers.Structured.Domain.Models;
using GenericHandlers.Structured.CommandHandlers.Authors.AddAuthor;

namespace UnitTests.StructuredHandlers;

public class AddAuthorValidatorTests
{
    private readonly AddAuthorValidator _validator = new();

    [Fact]
    public void Validate_IsSuccess()
    {
        var result = _validator.Validate(
            new AddAuthorData(null, "Dr.", "Seuss"));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_IsFailure()
    {
        var result = _validator.Validate(
            new AddAuthorData(new Author(1, "Dr.", "Seuss"), "Dr.", "Seuss"));

        result.IsValid.Should().BeFalse();
    }
}