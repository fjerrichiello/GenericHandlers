using Common.DataFactory;
using Common.Operations;
using Common.Verifiers;
using Microsoft.Extensions.Logging;

namespace Common.Messaging;

public class PublishingEventHandler<TMessage, TUnverifiedData, TVerifiedData>(
    IDataFactory<TMessage,
        EventMetadata, TUnverifiedData, TVerifiedData> _dataFactory,
    IEventVerifier<TMessage, TUnverifiedData> _verifier,
    IEventPublishingOperation<TMessage, EventMetadata, TVerifiedData> _service,
    IEventPublisher _eventPublisher,
    ILogger<PublishingEventHandler<TMessage, TUnverifiedData, TVerifiedData>>
        _logger)
    : IMessageContainerHandler<TMessage,
        EventMetadata>
    where TMessage : Message
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