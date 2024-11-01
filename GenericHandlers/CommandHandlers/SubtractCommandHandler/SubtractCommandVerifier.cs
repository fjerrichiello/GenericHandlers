using Common.Events.SubtractCommand;
using Common.Messaging;
using Common.Verifiers;
using FluentValidation;
using FluentValidation.Results;
using GenericHandlers.Commands;

namespace GenericHandlers.CommandHandlers.SubtractCommandHandler;

public class SubtractCommandVerifier : AuthorizedCommandVerifier<SubtractCommand, SubtractCommandUnverifiedData,
    SubtractCommandAuthorizationFailedEvent, SubtractCommandValidationFailedEvent>
{
    protected override void AuthorizationRules()
    {
        RuleFor(x => x.MessageContainer.Message)
            .NotNull();
    }

    protected override void ValidationRules()
    {
        RuleFor(x => x.DataFactoryResult.Value1)
            .GreaterThan(0);
    }

    public override SubtractCommandAuthorizationFailedEvent CreateAuthorizationFailedEvent(
        MessageVerificationParameters<SubtractCommand, CommandMetadata, SubtractCommandUnverifiedData> parameters,
        AuthorizationResult authorizationResult)
    {
        return new SubtractCommandAuthorizationFailedEvent(authorizationResult.ErrorMessages);
    }

    public override SubtractCommandValidationFailedEvent CreateValidationFailedEvent(
        MessageVerificationParameters<SubtractCommand, CommandMetadata, SubtractCommandUnverifiedData> parameters,
        ValidationResult validationResult)
    {
        return new SubtractCommandValidationFailedEvent(validationResult.ToDictionary());
    }
}