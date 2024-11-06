using Common.Events.AddAuthorCommand;
using Common.Messaging;
using GenericHandlers.CommandHandlers.Authors.AddAuthor;
using GenericHandlers.Commands.Authors;
using GenericHandlers.Domain.Models;
using GenericHandlers.Persistence.Repositories;
using GenericHandlers.Persistence.UnitOfWork;
using Moq;

namespace UnitTests.GenericHandlers;

public class AddAuthorOperationTests
{
    private readonly AddAuthorOperation _operation;

    private readonly Mock<IAuthorRepository> _mockRepo = new();
    private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
    private readonly Mock<IEventPublisher> _mockEventPublisher = new();

    public AddAuthorOperationTests()
    {
        _mockRepo.Reset();
        _mockUnitOfWork.Reset();
        _mockEventPublisher.Reset();

        _operation = new AddAuthorOperation(_mockRepo.Object, _mockUnitOfWork.Object, _mockEventPublisher.Object);
    }


    [Fact]
    public async Task AddAuthor_IsSuccess()
    {
        var messageContainer =
            new MessageContainer<AddAuthorCommand, CommandMetadata>(new AddAuthorCommand("Dr.", "Seuss"), new());

        var verifiedData = new AddAuthorVerifiedData("Dr.", "Seuss");

        await _operation.ExecuteAsync(messageContainer, verifiedData);

        _mockRepo.Verify(mock => mock.AddAsync(It.IsAny<Author>()), Times.Once);

        _mockUnitOfWork.Verify(mock => mock.CompleteAsync(), Times.Once);

        _mockEventPublisher.Verify(
            mock => mock.PublishAsync(messageContainer, It.IsAny<IEnumerable<AuthorAddedEvent>>()),
            Times.Once);
    }
}