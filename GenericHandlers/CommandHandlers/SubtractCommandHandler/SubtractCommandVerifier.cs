using Common.Events.SubtractCommand;
using Common.Messaging;
using Common.Verifiers;
using FluentValidation;
using FluentValidation.Results;
using GenericHandlers.Commands;

namespace GenericHandlers.CommandHandlers.SubtractCommandHandler;

public class SubtractCommandVerifier : AuthorizedCommandVerifier<SubtractCommand, SubtractCommandUnverifiedData>
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
}