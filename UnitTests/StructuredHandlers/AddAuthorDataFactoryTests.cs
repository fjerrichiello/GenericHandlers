using Common.Structured.Messaging;
using FluentAssertions;
using GenericHandlers.Domain.Models;
using GenericHandlers.Persistence.Repositories;
using GenericHandlers.StructuredCommandHandlers.Authors.AddAuthor;
using GenericHandlers.StructuredCommands.Authors;
using Moq;

namespace UnitTests.StructuredHandlers;

public class AddAuthorDataFactoryTests
{
    private readonly AddAuthorDataFactory _dataFactory;

    private readonly Mock<IAuthorRepository> _mockRepo = new();

    public AddAuthorDataFactoryTests()
    {
        _mockRepo.Reset();

        _dataFactory = new AddAuthorDataFactory(_mockRepo.Object);
    }

    [Fact]
    public async Task GetDataAsync_IsSuccess()
    {
        var messageContainer =
            new MessageContainer<AddAuthorCommand, CommandMetadata>(new AddAuthorCommand("Dr.", "Seuss"), new());

        _mockRepo.Setup(mock => mock.GetAsync(messageContainer.Message.FirstName, messageContainer.Message.LastName))
            .ReturnsAsync(new Author(1, "Dr.", "Seuss"));

        var result = await _dataFactory.GetDataAsync(messageContainer);

        result.Author.Should().NotBeNull();
        result.FirstName.Should().Be(messageContainer.Message.FirstName);
        result.LastName.Should().Be(messageContainer.Message.LastName);
    }
}