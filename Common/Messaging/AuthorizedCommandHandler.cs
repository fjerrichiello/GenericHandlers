﻿using Common.DataFactory;
using Common.Operations;
using Common.Verifiers;
using Microsoft.Extensions.Logging;

namespace Common.Messaging;

public class AuthorizedCommandHandler<TMessage, TUnverifiedData, TVerifiedData, TAuthorizationFailedEvent,
    TValidationFailedEvent, TFailedEvent>(
    IDataFactory<TMessage,
        CommandMetadata, TUnverifiedData, TVerifiedData> _dataFactory,
    IAuthorizedCommandVerifier<TMessage, TUnverifiedData, TAuthorizationFailedEvent, TValidationFailedEvent> _verifier,
    IPublishingOperation<TMessage, CommandMetadata, TVerifiedData, TFailedEvent> _service,
    IEventPublisher _eventPublisher,
    ILogger<AuthorizedCommandHandler<TMessage, TUnverifiedData, TVerifiedData, TAuthorizationFailedEvent,
        TValidationFailedEvent, TFailedEvent>> _logger)
    : IMessageContainerHandler<TMessage,
        CommandMetadata>
    where TMessage : Message
    where TAuthorizationFailedEvent : Message
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

            var authorizationResult = _verifier.Authorize(verificationParameters);
            if (!authorizationResult.IsAuthorized)
            {
                await _eventPublisher.PublishAsync(container,
                    _verifier.CreateAuthorizationFailedEvent(verificationParameters, authorizationResult));
                return;
            }

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
            await _eventPublisher.PublishAsync(container, _service.CreateFailedEvent(container, e));
        }
    }
}