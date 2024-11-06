using FluentAssertions;
using GenericHandlers.Structured.CommandHandlers.Authors.AddAuthor;

namespace UnitTests.StructuredHandlers;

public class AddAuthorAuthorizerTests
{
    private readonly AddAuthorAuthorizer _authorizer = new();

    [Fact]
    public void Authorize_IsSuccess()
    {
        var result = _authorizer.Authorize(new AddAuthorData(null, "Dr.", "Seuss"));

        result.IsAuthorized.Should().BeTrue();
    }
}