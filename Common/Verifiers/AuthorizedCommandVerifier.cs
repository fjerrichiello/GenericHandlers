using Common.Messaging;
using FluentValidation;
using FluentValidation.Results;

namespace Common.Verifiers;

public abstract class AuthorizedCommandVerifier<TCommand, TUnverifiedData, TAuthorizationFailedEvent,
    TValidationFailedEvent> :
    AbstractValidator<MessageVerificationParameters<TCommand, CommandMetadata, TUnverifiedData>>,
    IAuthorizedCommandVerifier<TCommand, TUnverifiedData, TAuthorizationFailedEvent,
        TValidationFailedEvent>
    where TCommand : Message where TAuthorizationFailedEvent : Message where TValidationFailedEvent : Message
{
    protected AuthorizedCommandVerifier()
    {
        RuleSet("Authorize", AuthorizationRules);
        RuleSet("Validate", ValidationRules);
    }

    protected abstract void AuthorizationRules();
    protected abstract void ValidationRules();

    public AuthorizationResult Authorize(
        MessageVerificationParameters<TCommand, CommandMetadata, TUnverifiedData> parameters)
    {
        var authorizationResult = new AuthorizationResult();
        var result = this.Validate(parameters, options => options.IncludeRuleSets("Authorize"));

        if (!result.IsValid)
        {
            authorizationResult.AddError("Does not have authorization");
        }

        return authorizationResult;
    }

    ValidationResult IAuthorizedCommandVerifier<TCommand, TUnverifiedData, TAuthorizationFailedEvent,
        TValidationFailedEvent>.Validate(
        MessageVerificationParameters<TCommand, CommandMetadata, TUnverifiedData> parameters)
    {
        return this.Validate(parameters, options => options.IncludeRuleSets("Validate"));
    }

    public abstract TAuthorizationFailedEvent CreateAuthorizationFailedEvent(
        MessageVerificationParameters<TCommand, CommandMetadata, TUnverifiedData> parameters,
        AuthorizationResult authorizationResult);

    public abstract TValidationFailedEvent CreateValidationFailedEvent(
        MessageVerificationParameters<TCommand, CommandMetadata, TUnverifiedData> parameters,
        ValidationResult validationResult);
}