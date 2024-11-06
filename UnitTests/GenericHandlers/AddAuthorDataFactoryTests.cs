using Common.Messaging;
using FluentAssertions;
using GenericHandlers.Domain.Models;
using GenericHandlers.Persistence.Repositories;
using GenericHandlers.CommandHandlers.Authors.AddAuthor;
using GenericHandlers.Commands.Authors;
using Moq;

namespace UnitTests.GenericHandlers;

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

        var result = await _dataFactory.GetUnverifiedDataAsync(messageContainer);

        result.Author.Should().NotBeNull();
        result.FirstName.Should().Be(messageContainer.Message.FirstName);
        result.LastName.Should().Be(messageContainer.Message.LastName);
    }

    [Fact]
    public void GetVerifiedDataAsync_IsSuccess()
    {
        var unverifiedData = new AddAuthorUnverifiedData(null, "Dr.", "Seuss");

        var result = _dataFactory.GetVerifiedData(unverifiedData);

        result.FirstName.Should().Be(unverifiedData.FirstName);
        result.LastName.Should().Be(unverifiedData.LastName);
    }
}