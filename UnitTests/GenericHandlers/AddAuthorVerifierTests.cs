using Common.Messaging;
using Common.Verifiers;
using FluentAssertions;
using GenericHandlers.CommandHandlers.Authors.AddAuthor;
using GenericHandlers.Commands.Authors;
using GenericHandlers.Domain.Models;

namespace UnitTests.GenericHandlers;

public class AddAuthorVerifierTests
{
    private readonly AddAuthorVerifier _verifier = new();

    [Fact]
    public void Authorize_IsSuccess()
    {
        var messageContainer =
            new MessageContainer<AddAuthorCommand, CommandMetadata>(new AddAuthorCommand("Dr.", "Seuss"), new());

        var result = _verifier.Authorize(
            new MessageVerificationParameters<AddAuthorCommand, CommandMetadata, AddAuthorUnverifiedData>(
                messageContainer, new AddAuthorUnverifiedData(null, "Dr.", "Seuss")));


        result.IsAuthorized.Should().BeTrue();
    }

    [Fact]
    public void Validate_IsSuccess()
    {
        var messageContainer =
            new MessageContainer<AddAuthorCommand, CommandMetadata>(new AddAuthorCommand("Dr.", "Seuss"), new());

        var result = _verifier.ValidateInternal(
            new MessageVerificationParameters<AddAuthorCommand, CommandMetadata, AddAuthorUnverifiedData>(
                messageContainer, new AddAuthorUnverifiedData(null, "Dr.", "Seuss")));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_IsFailure()
    {
        var messageContainer =
            new MessageContainer<AddAuthorCommand, CommandMetadata>(new AddAuthorCommand("Dr.", "Seuss"), new());

        var result = _verifier.ValidateInternal(
            new MessageVerificationParameters<AddAuthorCommand, CommandMetadata, AddAuthorUnverifiedData>(
                messageContainer, new AddAuthorUnverifiedData(new Author(1, "Dr.", "Seuss"), "Dr.", "Seuss")));

        result.IsValid.Should().BeFalse();
    }
}