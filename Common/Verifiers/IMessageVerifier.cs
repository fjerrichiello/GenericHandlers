using Common.Messaging;
using FluentValidation.Results;

namespace Common.Verifiers;

public interface IMessageVerifier<TMessage, TMessageMetadata, TUnverified>
    where TMessage : Message
    where TMessageMetadata : MessageMetadata
{
    ValidationResult Validate(MessageVerificationParameters<TMessage, TMessageMetadata, TUnverified> parameters);
}