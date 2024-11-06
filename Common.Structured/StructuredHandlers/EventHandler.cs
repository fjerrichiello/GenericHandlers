using Common.Structured.DataFactory;
using Common.Structured.Messaging;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace Common.Structured.StructuredHandlers;

public abstract class EventHandler<TMessage, TData>(
    IDataFactory<TMessage, EventMetadata, TData> _dataFactory,
    IValidator<TData> _validator,
    ILogger<EventHandler<TMessage, TData>> _logger) : IMessageContainerHandler<TMessage, EventMetadata>
    where TMessage : Message

{
    public async Task HandleAsync(MessageContainer<TMessage, EventMetadata> container)
    {
        try
        {
            var data = await CreateData(container);

            var isValid = await Validate(data);
            if (!isValid.IsValid)
            {
                return;
            }

            await Process(container, data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
        }
    }

    protected virtual async Task<TData> CreateData(MessageContainer<TMessage, EventMetadata> container)
    {
        return await _dataFactory.GetDataAsync(container);
    }


    protected virtual async Task<ValidationResult> Validate(TData data)
        => _validator.Validate(data);


    protected abstract Task Process(MessageContainer<TMessage, EventMetadata> commandContainer, TData data);

// protected StructuredCommandHandler(IDataFactory<TData> dataFactory,
//     IValidator<TData> vaildator, IAuthorizer<TData> authorizer, IEventPublisher eventPublisher)
// {
//     this.dataFactory = dataFactory;
//     this.vaildator = vaildator;
//     this.authorizer = authorizer;
//     this.eventPublisher = eventPublisher;
// }
}