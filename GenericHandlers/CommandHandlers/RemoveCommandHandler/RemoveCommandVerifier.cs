using Common.Events.RemoveCommand;
using Common.Messaging;
using Common.Verifiers;
using FluentValidation;
using FluentValidation.Results;
using GenericHandlers.Commands;

namespace GenericHandlers.CommandHandlers.RemoveCommandHandler;

public class RemoveCommandVerifier : MessageVerifier<RemoveCommand, CommandMetadata, RemoveCommandUnverifiedData,
    RemoveCommandValidationFailedEvent>
{
    protected override void ValidationRules()
    {
        RuleFor(x => x.DataFactoryResult.Value1)
            .GreaterThan(-1);
    }

    public override RemoveCommandValidationFailedEvent CreateValidationFailedEvent(
        MessageVerificationParameters<RemoveCommand, CommandMetadata, RemoveCommandUnverifiedData> parameters,
        ValidationResult validationResult)
    {
        return new RemoveCommandValidationFailedEvent(validationResult.ToDictionary());
    }
}