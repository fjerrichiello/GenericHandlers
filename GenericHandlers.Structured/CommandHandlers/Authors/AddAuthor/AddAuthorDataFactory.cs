using Common.Structured.DataFactory;
using Common.Structured.Messaging;
using GenericHandlers.Structured.Commands.Authors;
using GenericHandlers.Structured.Persistence.Repositories;

namespace GenericHandlers.Structured.CommandHandlers.Authors.AddAuthor;

public class AddAuthorDataFactory(IAuthorRepository _authorRepository)
    : IDataFactory<AddAuthorCommand, CommandMetadata, AddAuthorData>
{
    public async Task<AddAuthorData> GetDataAsync(MessageContainer<AddAuthorCommand, CommandMetadata> container)
    {
        var author = await _authorRepository.GetAsync(container.Message.FirstName, container.Message.LastName);

        return new AddAuthorData(author, container.Message.FirstName, container.Message.LastName);
    }
}