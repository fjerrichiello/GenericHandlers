using Common.Helpers;
using Common.Messaging;
using FluentValidation;
using FluentValidation.Results;

namespace Common.Verifiers;

public abstract class AuthorizedCommandVerifier<TCommand, TUnverifiedData> :
    AbstractValidator<MessageVerificationParameters<TCommand, CommandMetadata, TUnverifiedData>>,
    IAuthorizedCommandVerifier<TCommand, TUnverifiedData>
    where TCommand : Message
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

    ValidationResult IAuthorizedCommandVerifier<TCommand, TUnverifiedData>.Validate(
        MessageVerificationParameters<TCommand, CommandMetadata, TUnverifiedData> parameters)
    {
        return this.Validate(parameters, options => options.IncludeRuleSets("Validate"));
    }
}