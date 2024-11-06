using Common.Structured.Authorization;
using Common.Structured.DataFactory;
using Common.Structured.Messaging;
using Common.Structured.Messaging.Publishing;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;

namespace Common.Structured.StructuredHandlers;

public abstract class AuthorizedCommandHandler<TMessage, TData>(
    IDataFactory<TMessage, CommandMetadata, TData> _dataFactory,
    IAuthorizer<TData> _authorizer,
    IValidator<TData> _validator,
    IEventPublisher _eventPublisher,
    ILogger<AuthorizedCommandHandler<TMessage, TData>> _logger) : IMessageContainerHandler<TMessage, CommandMetadata>
    where TMessage : Message

{
    public async Task HandleAsync(MessageContainer<TMessage, CommandMetadata> container)
    {
        try
        {
            var data = await CreateData(container);

            var isAuthorized = await Authorize(data);
            if (!isAuthorized.IsAuthorized)
            {
                await _eventPublisher.PublishAuthorizationFailedAsync(container,
                    new AuthorizationFailedEvent(isAuthorized.ErrorMessage));
                return;
            }

            var isValid = await Validate(data);
            if (!isValid.IsValid)
            {
                await _eventPublisher.PublishValidationFailedAsync(container,
                    new ValidationFailedEvent(new Dictionary<string, string[]>()));
                return;
            }


            await Process(container, data);
        }
        catch (Exception ex)
        {
            await _eventPublisher.PublishUnhandledExceptionAsync(container, new UnhandledExceptionEvent(ex.Message));
            _logger.LogError(ex.Message);
        }
    }

    protected virtual async Task<TData> CreateData(MessageContainer<TMessage, CommandMetadata> container)
    {
        return await _dataFactory.GetDataAsync(container);
    }


    protected virtual async Task<AuthorizationResult> Authorize(
        TData data)
        => _authorizer.Authorize(data);


    protected virtual async Task<ValidationResult> Validate(TData data)
        => _validator.Validate(data);


    public abstract Task Process(MessageContainer<TMessage, CommandMetadata> commandContainer, TData data);

// protected StructuredCommandHandler(IDataFactory<TData> dataFactory,
//     IValidator<TData> vaildator, IAuthorizer<TData> authorizer, IEventPublisher eventPublisher)
// {
//     this.dataFactory = dataFactory;
//     this.vaildator = vaildator;
//     this.authorizer = authorizer;
//     this.eventPublisher = eventPublisher;
// }
}