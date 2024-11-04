using Common.Messaging;
using FluentValidation.Results;

namespace Common.Verifiers;

public interface IAuthorizedCommandVerifier<TCommand, TUnverifiedData>
    where TCommand : Message
{
    AuthorizationResult Authorize(
        MessageVerificationParameters<TCommand, CommandMetadata, TUnverifiedData> parameters);

    ValidationResult Validate(MessageVerificationParameters<TCommand, CommandMetadata, TUnverifiedData> parameters);
}