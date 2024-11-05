using Common.Events.AddAuthorCommand;
using Common.Messaging;
using Common.Operations;
using GenericHandlers.Commands.Authors;
using GenericHandlers.Domain.Models;
using GenericHandlers.Persistence.Repositories;

namespace GenericHandlers.CommandHandlers.Authors.AddAuthor;

public class AddAuthorOperation(IAuthorRepository _authorRepository, IEventPublisher _eventPublisher)
    : IOperation<AddAuthorCommand, CommandMetadata, AddAuthorVerifiedData>
{
    public async Task ExecuteAsync(MessageContainer<AddAuthorCommand, CommandMetadata> container,
        AddAuthorVerifiedData data)
    {
        var author = new Author(Random.Shared.Next(1000000), data.FirstName, data.LastName);

        await _authorRepository.AddAsync(author);

        await _eventPublisher.PublishAsync(container,
            new AuthorAddedEvent(author.Id, author.FirstName, author.LastName));
    }
}