using Common.Messaging;
using FluentValidation.Results;

namespace Common.EventFactory;

public class CommandEventFactory<TValidationFailedEvent> : ICommandEventFactory<TValidationFailedEvent>
    where TValidationFailedEvent : ValidationFailedMessage, new()
{
    public TValidationFailedEvent CreateFailedValidationEvent(ValidationResult validationResult)
        => new()
        {
            Errors = validationResult.ToDictionary()
        };
}