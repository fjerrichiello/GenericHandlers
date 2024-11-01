using CommonWithEventFactories.Events.MultipliedCommand;
using CommonWithEventFactories.Verifiers;
using FluentValidation;

namespace GenericHandlersWithEventFactories.EventHandlers.MultipliedEventHandler;

public class MultipliedEventVerifier : EventVerifier<MultipliedEvent, MultipliedEventUnverifiedData>
{
    protected override void ValidationRules()
    {
        RuleFor(x => x.DataFactoryResult.Value1)
            .GreaterThan(-1);
    }
}