using Common.Events.AddAuthorCommandStructured;
using Common.Structured.Authorization;
using Common.Structured.DataFactory;
using Common.Structured.Messaging;
using FluentValidation;
using FluentValidation.Results;
using GenericHandlers.CommandHandlers.Authors.AddAuthor;
using GenericHandlers.Domain.Models;
using GenericHandlers.Persistence.Repositories;
using GenericHandlers.Persistence.UnitOfWork;
using GenericHandlers.StructuredCommandHandlers.Authors.AddAuthor;
using GenericHandlers.StructuredCommands.Authors;
using Microsoft.Extensions.Logging;
using Moq;

namespace UnitTests.StructuredHandlers;

public class AddAuthorCommandHandlerTests
{
    private readonly AddAuthorCommandHandler _commandHandler;
    private readonly Mock<IDataFactory<AddAuthorCommand, CommandMetadata, AddAuthorData>> _mockDataFactory = new();
    private readonly Mock<IAuthorizer<AddAuthorData>> _mockAuthorizer = new();
    private readonly Mock<IValidator<AddAuthorData>> _mockValidator = new();
    private readonly Mock<ILogger<AddAuthorCommandHandler>> _mockLogger = new();
    private readonly Mock<IAuthorRepository> _mockRepo = new();

    private readonly Mock<IUnitOfWork> _mockUnitOfWork = new();
    private readonly Mock<IEventPublisher> _mockEventPublisher = new();

    public AddAuthorCommandHandlerTests()
    {
        _mockRepo.Reset();
        _mockUnitOfWork.Reset();
        _mockEventPublisher.Reset();

        _commandHandler =
            new AddAuthorCommandHandler(_mockDataFactory.Object, _mockAuthorizer.Object, _mockValidator.Object,
                _mockEventPublisher.Object, _mockLogger.Object,
                _mockRepo.Object, _mockUnitOfWork.Object);
    }


    [Fact]
    public async Task AddAuthor_IsSuccess()
    {
        var messageContainer =
            new MessageContainer<AddAuthorCommand, CommandMetadata>(new AddAuthorCommand("Dr.", "Seuss"), new());

        var data = new AddAuthorData(null, "Dr.", "Seuss");

        _mockDataFactory.Setup(mock => mock.GetDataAsync(messageContainer)).ReturnsAsync(data);

        _mockAuthorizer.Setup(mock => mock.Authorize(data)).Returns(new AuthorizationResult());

        _mockValidator.Setup(mock => mock.Validate(data)).Returns(new ValidationResult());

        await _commandHandler.HandleAsync(messageContainer);

        _mockRepo.Verify(mock => mock.AddAsync(It.IsAny<Author>()), Times.Once);

        _mockUnitOfWork.Verify(mock => mock.CompleteAsync(), Times.Once);

        _mockEventPublisher.Verify(
            mock => mock.PublishAsync(messageContainer, It.IsAny<IEnumerable<AuthorAddedEvent>>()),
            Times.Once);
    }
}