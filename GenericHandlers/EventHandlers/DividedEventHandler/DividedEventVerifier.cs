using Common.Events.DividedCommand;
using Common.Events.MultipliedCommand;
using Common.Messaging;
using Common.Verifiers;
using FluentValidation;

namespace GenericHandlers.EventHandlers.DividedEventHandler;

public class DividedEventVerifier : MessageVerifier<DividedEvent, EventMetadata, DividedEventUnverifiedData>
{
    protected override void ValidationRules()
    {
        RuleFor(x => x.DataFactoryResult.Value1)
            .GreaterThan(-1);
    }
}