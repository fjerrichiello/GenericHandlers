using Common.DataFactory;
using Common.Helpers;
using Common.Messaging;
using Common.Operations;
using Common.Verifiers;
using Microsoft.Extensions.Logging;

namespace Common.DefaultHandlers;

public class CommandHandler<TMessage, TUnverifiedData, TVerifiedData, TFailedEvent>(
    IDataFactory<TMessage,
        CommandMetadata, TUnverifiedData, TVerifiedData> _dataFactory,
    IMessageVerifier<TMessage, CommandMetadata, TUnverifiedData> _verifier,
    IPublishingOperation<TMessage, CommandMetadata, TVerifiedData, TFailedEvent> _operation,
    IEventPublisher _eventPublisher,
    ILogger<CommandHandler<TMessage, TUnverifiedData, TVerifiedData, TFailedEvent>>
        _logger)
    : IMessageContainerHandler<TMessage,
        CommandMetadata>
    where TMessage : Message
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
                    new ValidationFailedMessage(validationResult.ToDictionarySnakeCase()));
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