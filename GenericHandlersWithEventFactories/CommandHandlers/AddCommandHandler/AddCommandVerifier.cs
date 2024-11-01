using CommonWithEventFactories.Events.AddCommand;
using CommonWithEventFactories.Messaging;
using CommonWithEventFactories.Verifiers;
using FluentValidation;
using GenericHandlersWithEventFactories.Commands;

namespace GenericHandlersWithEventFactories.CommandHandlers.AddCommandHandler;

public class AddCommandVerifier : AuthorizedCommandVerifier<AddCommand, AddCommandUnverifiedData>
{
    protected override void AuthorizationRules()
    {
        RuleFor(x => x.MessageContainer.Message)
            .NotEmpty();
    }

    protected override void ValidationRules()
    {
        RuleFor(x => x.DataFactoryResult.Value1)
            .GreaterThan(0);
    }
}