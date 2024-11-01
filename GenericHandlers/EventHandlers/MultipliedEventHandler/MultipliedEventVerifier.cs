using Common.Events.MultipliedCommand;
using Common.Verifiers;
using FluentValidation;

namespace GenericHandlers.EventHandlers.MultipliedEventHandler;

public class MultipliedEventVerifier : EventVerifier<MultipliedEvent, MultipliedEventUnverifiedData>
{
    protected override void ValidationRules()
    {
        RuleFor(x => x.DataFactoryResult.Value1)
            .GreaterThan(-1);
    }
}