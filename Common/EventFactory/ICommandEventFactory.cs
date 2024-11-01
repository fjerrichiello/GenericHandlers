using Common.Verifiers;
using FluentValidation.Results;

namespace Common.EventFactory;

public interface ICommandEventFactory<TValidationFailedEvent>
{
    public TValidationFailedEvent CreateFailedValidationEvent(ValidationResult validationResult);
}