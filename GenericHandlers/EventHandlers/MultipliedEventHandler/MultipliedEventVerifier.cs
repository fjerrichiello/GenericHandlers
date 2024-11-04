using Common.Events.MultiplyCommand;
using Common.Messaging;
using Common.Verifiers;
using FluentValidation;

namespace GenericHandlers.EventHandlers.MultipliedEventHandler;

public class MultipliedEventVerifier : MessageVerifier<MultipliedEvent, EventMetadata, MultipliedEventUnverifiedData>
{
    protected override void ValidationRules()
    {
        RuleFor(x => x.DataFactoryResult.Value1)
            .GreaterThan(-1);
    }
}