using CommonWithEventFactories.DataFactory;
using CommonWithEventFactories.EventFactory;
using CommonWithEventFactories.Messaging;
using CommonWithEventFactories.Operations;
using CommonWithEventFactories.Verifiers;
using Microsoft.Extensions.Logging;

namespace CommonWithEventFactories.DefaultHandlers;

public class CommandHandler<TMessage, TUnverifiedData, TVerifiedData,
    TValidationFailedEvent, TFailedEvent>(
    IDataFactory<TMessage,
        CommandMetadata, TUnverifiedData, TVerifiedData> _dataFactory,
    IMessageVerifier<TMessage, CommandMetadata, TUnverifiedData> _verifier,
    IPublishingOperation<TMessage, CommandMetadata, TVerifiedData, TFailedEvent> _operation,
    ICommandEventFactory<TValidationFailedEvent> _eventFactory,
    IEventPublisher _eventPublisher,
    ILogger<CommandHandler<TMessage, TUnverifiedData, TVerifiedData, TValidationFailedEvent, TFailedEvent>>
        _logger)
    : IMessageContainerHandler<TMessage,
        CommandMetadata>
    where TMessage : Message
    where TValidationFailedEvent : Message
    where TFailedEvent : Message
{
    public async Task HandleAsync(MessageContainer<TMessage, CommandMetadata> container)
    {
        try
        {
            var unverifiedData = await _dataFactory.GetDataAsync(container);

            var verificationParameters =
                new MessageVerificationParameters<TMessage, CommandMetadata, TUnverifiedData>(container,
                    unverifiedData);

            var validationResult = _verifier.Validate(verificationParameters);
            if (!validationResult.IsValid)
            {
                await _eventPublisher.PublishAsync(container,
                    _eventFactory.CreateFailedValidationEvent(validationResult));
                return;
            }

            var verifiedData = _dataFactory.GetVerifiedData(unverifiedData);

            await _operation.ExecuteAsync(container, verifiedData, _eventPublisher);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            await _eventPublisher.PublishAsync(container, _operation.CreateFailedEvent(container, e));
        }
    }
}