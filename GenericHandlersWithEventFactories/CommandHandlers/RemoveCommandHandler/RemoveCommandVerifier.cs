using CommonWithEventFactories.Events.RemoveCommand;
using CommonWithEventFactories.Messaging;
using CommonWithEventFactories.Verifiers;
using FluentValidation;
using GenericHandlersWithEventFactories.Commands;

namespace GenericHandlersWithEventFactories.CommandHandlers.RemoveCommandHandler;

public class RemoveCommandVerifier : MessageVerifier<RemoveCommand, CommandMetadata, RemoveCommandUnverifiedData>
{
    protected override void ValidationRules()
    {
        RuleFor(x => x.DataFactoryResult.Value1)
            .GreaterThan(-1);
    }

   
}