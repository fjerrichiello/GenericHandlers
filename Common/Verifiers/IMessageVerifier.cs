using Common.Messaging;
using FluentValidation.Results;

namespace Common.Verifiers;

public interface IMessageVerifier<TMessage, TMessageMetadata, TUnverified,
    out TValidationFailedEvent>
    where TMessage : Message
    where TMessageMetadata : MessageMetadata
    where TValidationFailedEvent : Message
{
    ValidationResult Validate(MessageVerificationParameters<TMessage, TMessageMetadata, TUnverified> parameters);

    TValidationFailedEvent CreateValidationFailedEvent(
        MessageVerificationParameters<TMessage, TMessageMetadata, TUnverified> parameters,
        ValidationResult validationResult);
}