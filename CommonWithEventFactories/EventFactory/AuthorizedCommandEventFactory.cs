﻿
using CommonWithEventFactories.Messaging;
using CommonWithEventFactories.Verifiers;
using FluentValidation.Results;

namespace CommonWithEventFactories.EventFactory;

public class
    AuthorizedCommandEventFactory<TAuthorizationFailedEvent, TValidationFailedEvent> : IAuthorizedCommandEventFactory<
    TAuthorizationFailedEvent, TValidationFailedEvent>
    where TAuthorizationFailedEvent : AuthorizationFailedMessage, new()
    where TValidationFailedEvent : ValidationFailedMessage, new()
{
    public TAuthorizationFailedEvent CreateFailedAuthorizationEvent(AuthorizationResult authorizationResult)
        => new()
        {
            Reason = authorizationResult.ErrorMessages
        };

    public TValidationFailedEvent CreateFailedValidationEvent(ValidationResult validationResult) =>
        new()
        {
            Errors = validationResult.ToDictionary()
        };
}