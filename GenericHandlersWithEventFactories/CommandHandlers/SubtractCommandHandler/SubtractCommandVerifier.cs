using CommonWithEventFactories.Events.SubtractCommand;
using CommonWithEventFactories.Messaging;
using CommonWithEventFactories.Verifiers;
using FluentValidation;
using GenericHandlersWithEventFactories.Commands;

namespace GenericHandlersWithEventFactories.CommandHandlers.SubtractCommandHandler;

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