using Common.DataFactory;
using Common.Helpers;
using Common.Messaging;
using Common.Operations;
using Common.Verifiers;
using Microsoft.Extensions.Logging;

namespace Common.DefaultHandlers;

public class AuthorizedCommandHandler<TMessage, TUnverifiedData, TVerifiedData>(
    IDataFactory<TMessage,
        CommandMetadata, TUnverifiedData, TVerifiedData> _dataFactory,
    IAuthorizedMessageVerifier<TMessage, CommandMetadata, TUnverifiedData> _verifier,
    IOperation<TMessage, CommandMetadata, TVerifiedData> _operation,
    IEventPublisher _eventPublisher,
    ILogger<AuthorizedCommandHandler<TMessage, TUnverifiedData, TVerifiedData>> _logger)
    : IMessageContainerHandler<TMessage,
        CommandMetadata>
    where TMessage : Message
{
    public async Task HandleAsync(MessageContainer<TMessage, CommandMetadata> container)
    {
        try
        {
            var unverifiedData = await _dataFactory.GetDataAsync(container);

            var verificationParameters =
                new MessageVerificationParameters<TMessage, CommandMetadata, TUnverifiedData>(container,
                    unverifiedData);

            var authorizationResult = _verifier.Authorize(verificationParameters);
            if (!authorizationResult.IsAuthorized)
            {
                await _eventPublisher.PublishAsync(container,
                    new AuthorizationFailedEvent(authorizationResult.ErrorMessages));
                return;
            }

            var validationResult = _verifier.Validate(verificationParameters);
            if (!validationResult.IsValid)
            {
                await _eventPublisher.PublishAsync(container,
                    new ValidationFailedEvent(validationResult.ToDictionarySnakeCase()));
                return;
            }

            var verifiedData = _dataFactory.GetVerifiedData(unverifiedData);

            await _operation.ExecuteAsync(container, verifiedData);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            await _eventPublisher.PublishAsync(container, new UnhandledExceptionEvent(e.Message));
        }
    }
}