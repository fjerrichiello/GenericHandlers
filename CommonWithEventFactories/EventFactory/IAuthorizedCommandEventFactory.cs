
using CommonWithEventFactories.Verifiers;
using FluentValidation.Results;

namespace CommonWithEventFactories.EventFactory;

public interface IAuthorizedCommandEventFactory<TAuthorizationFailedEvent, TValidationFailedEvent>
{
    public TAuthorizationFailedEvent CreateFailedAuthorizationEvent(AuthorizationResult authorizationResult);

    public TValidationFailedEvent CreateFailedValidationEvent(ValidationResult validationResult);
}