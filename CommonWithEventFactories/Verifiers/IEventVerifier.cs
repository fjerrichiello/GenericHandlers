
using CommonWithEventFactories.Messaging;
using FluentValidation.Results;

namespace CommonWithEventFactories.Verifiers;

public interface IEventVerifier<TMessage, TUnverified>
    where TMessage : Message
{
    ValidationResult Validate(MessageVerificationParameters<TMessage, EventMetadata, TUnverified> parameters);
}