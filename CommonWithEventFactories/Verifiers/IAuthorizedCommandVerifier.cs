using CommonWithEventFactories.Messaging;
using FluentValidation.Results;

namespace CommonWithEventFactories.Verifiers;

public interface IAuthorizedCommandVerifier<TCommand, TUnverifiedData>
    where TCommand : Message
{
    AuthorizationResult Authorize(
        MessageVerificationParameters<TCommand, CommandMetadata, TUnverifiedData> parameters);

    ValidationResult Validate(MessageVerificationParameters<TCommand, CommandMetadata, TUnverifiedData> parameters);
}