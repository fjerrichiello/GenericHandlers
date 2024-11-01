
using CommonWithEventFactories.Messaging;
using FluentValidation.Results;

namespace CommonWithEventFactories.EventFactory;

public class CommandEventFactory<TValidationFailedEvent> : ICommandEventFactory<TValidationFailedEvent>
    where TValidationFailedEvent : ValidationFailedMessage, new()
{
    public TValidationFailedEvent CreateFailedValidationEvent(ValidationResult validationResult)
        => new()
        {
            Errors = validationResult.ToDictionary()
        };
}