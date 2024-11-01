

using FluentValidation.Results;

namespace CommonWithEventFactories.EventFactory;

public interface ICommandEventFactory<TValidationFailedEvent>
{
    public TValidationFailedEvent CreateFailedValidationEvent(ValidationResult validationResult);
}