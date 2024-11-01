using Common.DataFactory;
using Common.Operations;
using Common.Verifiers;
using Microsoft.Extensions.Logging;

namespace Common.Messaging;

public class EventHandlerWithPublisher<TMessage, TUnverifiedData, TVerifiedData,
    TValidationFailedEvent, TFailedEvent>(
    IDataFactory<TMessage,
        EventMetadata, TUnverifiedData, TVerifiedData> _dataFactory,
    IMessageVerifier<TMessage, EventMetadata, TUnverifiedData,
        TValidationFailedEvent> _verifier,
    IEventPublishingOperation<TMessage, TVerifiedData> _service,
    IEventPublisher _eventPublisher,
    ILogger<CommandHandler<TMessage, TUnverifiedData, TVerifiedData, TValidationFailedEvent, TFailedEvent>>
        _logger)
    : IMessageContainerHandler<TMessage,
        EventMetadata>
    where TMessage : Message
    where TValidationFailedEvent : Message
    where TFailedEvent : Message
{
    public async Task HandleAsync(MessageContainer<TMessage, EventMetadata> container)
    {
        try
        {
            var unverifiedData = await _dataFactory.GetDataAsync(container);

            var verificationParameters =
                new MessageVerificationParameters<TMessage, EventMetadata, TUnverifiedData>(container,
                    unverifiedData);

            var validationResult = _verifier.Validate(verificationParameters);
            if (!validationResult.IsValid)
            {
                await _eventPublisher.PublishAsync(container,
                    _verifier.CreateValidationFailedEvent(verificationParameters, validationResult));
                return;
            }

            var verifiedData = _dataFactory.GetVerifiedData(unverifiedData);

            await _service.ExecuteAsync(container, verifiedData, _eventPublisher);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }
}