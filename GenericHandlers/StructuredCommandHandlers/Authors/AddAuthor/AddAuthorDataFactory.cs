using Common.Structured.DataFactory;
using Common.Structured.Messaging;
using GenericHandlers.Persistence.Repositories;
using GenericHandlers.StructuredCommands.Authors;

namespace GenericHandlers.StructuredCommandHandlers.Authors.AddAuthor;

public class AddAuthorDataFactory(IAuthorRepository _authorRepository)
    : IDataFactory<AddAuthorCommand, CommandMetadata, AddAuthorData>
{
    public async Task<AddAuthorData> GetDataAsync(MessageContainer<AddAuthorCommand, CommandMetadata> container)
    {
        var author = await _authorRepository.GetAsync(container.Message.FirstName, container.Message.LastName);

        return new AddAuthorData(author, container.Message.FirstName, container.Message.LastName);
    }
}