using Common.Messaging;
using FluentValidation.Results;

namespace Common.Verifiers;

public interface IAuthorizedCommandVerifier<TCommand, TUnverifiedData, out TAuthorizationFailedEvent,
    out TValidationFailedEvent>
    where TCommand : Message
    where TAuthorizationFailedEvent : Message
    where TValidationFailedEvent : Message
{
    AuthorizationResult Authorize(
        MessageVerificationParameters<TCommand, CommandMetadata, TUnverifiedData> parameters);

    ValidationResult Validate(MessageVerificationParameters<TCommand, CommandMetadata, TUnverifiedData> parameters);

    TAuthorizationFailedEvent CreateAuthorizationFailedEvent(
        MessageVerificationParameters<TCommand, CommandMetadata, TUnverifiedData> parameters,
        AuthorizationResult authorizationResult);

    TValidationFailedEvent CreateValidationFailedEvent(
        MessageVerificationParameters<TCommand, CommandMetadata, TUnverifiedData> parameters,
        ValidationResult validationResult);
}