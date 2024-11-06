using Common.DataFactory;
using Common.Helpers;
using Common.Messaging;
using Common.Operations;
using Common.Verifiers;
using Microsoft.Extensions.Logging;

namespace Common.DefaultHandlers;

public sealed class CommandHandler<TMessage, TUnverifiedData, TVerifiedData>(
    IDataFactory<TMessage,
        CommandMetadata, TUnverifiedData, TVerifiedData> _dataFactory,
    IMessageVerifier<TMessage, CommandMetadata, TUnverifiedData> _verifier,
    IOperation<TMessage, CommandMetadata, TVerifiedData> _operation,
    IEventPublisher _eventPublisher,
    ILogger<CommandHandler<TMessage, TUnverifiedData, TVerifiedData>>
        _logger)
    : IMessageContainerHandler<TMessage,
        CommandMetadata>
    where TMessage : Message
{
    public async Task HandleAsync(MessageContainer<TMessage, CommandMetadata> container)
    {
        try
        {
            var unverifiedData = await _dataFactory.GetUnverifiedDataAsync(container);

            var verificationParameters =
                new MessageVerificationParameters<TMessage, CommandMetadata, TUnverifiedData>(container,
                    unverifiedData);

            var validationResult = _verifier.ValidateInternal(verificationParameters);
            if (!validationResult.IsValid)
            {
                await _eventPublisher.PublishValidationFailedAsync(container,
                    new(validationResult.ToDictionarySnakeCase()));
                return;
            }

            var verifiedData = _dataFactory.GetVerifiedData(unverifiedData);

            await _operation.ExecuteAsync(container, verifiedData);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            await _eventPublisher.PublishUnhandledExceptionAsync(container, new(e.Message));
        }
    }
}