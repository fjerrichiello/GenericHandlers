using Common.Events.Structured.AddAuthorCommand;
using Common.Structured.Authorization;
using Common.Structured.DataFactory;
using Common.Structured.Messaging;
using Common.Structured.StructuredHandlers;
using FluentValidation;
using GenericHandlers.Structured.Commands.Authors;
using GenericHandlers.Structured.Domain.Models;
using GenericHandlers.Structured.Persistence.Repositories;
using GenericHandlers.Structured.Persistence.UnitOfWork;

namespace GenericHandlers.Structured.CommandHandlers.Authors.AddAuthor;

public class AddAuthorCommandHandler(
    IDataFactory<AddAuthorCommand, CommandMetadata, AddAuthorData> _dataFactory,
    IAuthorizer<AddAuthorData> _authorizer,
    IValidator<AddAuthorData> _validator,
    IEventPublisher _eventPublisher,
    ILogger<AddAuthorCommandHandler> _logger,
    IAuthorRepository _authorRepository,
    IUnitOfWork _unitOfWork)
    : AuthorizedCommandHandler<AddAuthorCommand, AddAuthorData>(_dataFactory, _authorizer, _validator, _eventPublisher,
        _logger)
{
    public override async Task Process(MessageContainer<AddAuthorCommand, CommandMetadata> commandContainer,
        AddAuthorData data)
    {
        var author = new Author(Random.Shared.Next(1000000), data.FirstName, data.LastName);

        await _authorRepository.AddAsync(author);

        await _unitOfWork.CompleteAsync();

        await _eventPublisher.PublishAsync(commandContainer,
            new AuthorAddedEvent(author.Id, author.FirstName, author.LastName));
    }
}