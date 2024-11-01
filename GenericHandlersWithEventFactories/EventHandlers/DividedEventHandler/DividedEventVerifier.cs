using CommonWithEventFactories.Events.DividedCommand;
using CommonWithEventFactories.Verifiers;
using FluentValidation;

namespace GenericHandlersWithEventFactories.EventHandlers.DividedEventHandler;

public class DividedEventVerifier : EventVerifier<DividedEvent, DividedEventUnverifiedData>
{
    protected override void ValidationRules()
    {
        RuleFor(x => x.DataFactoryResult.Value1)
            .GreaterThan(-1);
    }
}