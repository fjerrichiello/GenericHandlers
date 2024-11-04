using Common.Events.AddCommand;
using Common.Messaging;
using Common.Verifiers;
using FluentValidation;
using FluentValidation.Results;
using GenericHandlers.Commands;

namespace GenericHandlers.CommandHandlers.AddCommandHandler;

public class AddCommandVerifier : AuthorizedCommandVerifier<AddCommand, AddCommandUnverifiedData,
    AddCommandAuthorizationFailedEvent, AddCommandValidationFailedEvent>
{
    protected override void AuthorizationRules()
    {
        RuleFor(x => x.MessageContainer.Message)
            .NotEmpty();
    }

    protected override void ValidationRules()
    {
        RuleFor(x => x.DataFactoryResult.Value1)
            .LessThan(0);
    }

    public override AddCommandAuthorizationFailedEvent CreateAuthorizationFailedEvent(
        MessageVerificationParameters<AddCommand, CommandMetadata, AddCommandUnverifiedData> parameters,
        AuthorizationResult authorizationResult)
    {
        return new AddCommandAuthorizationFailedEvent(authorizationResult.ErrorMessages);
    }

    public override AddCommandValidationFailedEvent CreateValidationFailedEvent(
        MessageVerificationParameters<AddCommand, CommandMetadata, AddCommandUnverifiedData> parameters,
        ValidationResult validationResult)
    {
        return new AddCommandValidationFailedEvent(validationResult.ToDictionary());
    }
}