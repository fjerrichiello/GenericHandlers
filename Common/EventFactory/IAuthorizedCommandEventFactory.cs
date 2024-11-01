using Common.Verifiers;
using FluentValidation.Results;

namespace Common.EventFactory;

public interface IAuthorizedCommandEventFactory<TAuthorizationFailedEvent, TValidationFailedEvent>
{
    public TAuthorizationFailedEvent CreateFailedAuthorizationEvent(AuthorizationResult authorizationResult);

    public TValidationFailedEvent CreateFailedValidationEvent(ValidationResult validationResult);
}