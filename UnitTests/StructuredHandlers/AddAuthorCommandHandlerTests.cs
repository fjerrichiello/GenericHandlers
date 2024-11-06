using Common.Events.Structured.AddAuthorCommand;
using Common.Structured.Messaging;
using GenericHandlers.Structured.Domain.Models;
using GenericHandlers.Structured.Persistence.Repositories;
using GenericHandlers.Structured.Persistence.UnitOfWork;
using GenericHandlers.Structured.CommandHandlers.Authors.AddAuthor;
using GenericHandlers.Structured.Commands.Authors;
using Moq;

namespace UnitTests.StructuredHandlers;

public class AddAuthorCommandHandlerTests : BaseCommandHandlerTests<AddAuthorCommand, CommandMetadata, AddAuthorData,
    AddAuthorCommandHandler>
{
    private readonly AddAuthorCommandHandler _commandHandler;

    private readonly Mock<IAuthorRepository> _mockRepo = new();
    private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
    private readonly Mock<IEventPublisher> _mockEventPublisher = new();

    public AddAuthorCommandHandlerTests()
    {
        _mockRepo.Reset();
        _mockUnitOfWork.Reset();
        _mockEventPublisher.Reset();


        _commandHandler =
            new AddAuthorCommandHandler(
                _dataFactory,
                _authorizer,
                _validator,
                _mockEventPublisher.Object,
                _logger,
                _mockRepo.Object,
                _mockUnitOfWork.Object);
    }


    [Fact]
    public async Task Process_IsSuccess()
    {
        var messageContainer =
            new MessageContainer<AddAuthorCommand, CommandMetadata>(new AddAuthorCommand("Dr.", "Seuss"), new());

        var data = new AddAuthorData(null, "Dr.", "Seuss");

        await _commandHandler.Process(messageContainer, data);

        _mockRepo.Verify(mock => mock.AddAsync(It.IsAny<Author>()), Times.Once);

        _mockUnitOfWork.Verify(mock => mock.CompleteAsync(), Times.Once);

        _mockEventPublisher.Verify(
            mock => mock.PublishAsync(messageContainer, It.IsAny<IEnumerable<AuthorAddedEvent>>()),
            Times.Once);
    }
}