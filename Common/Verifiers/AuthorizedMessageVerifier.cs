using Common.Messaging;
using FluentValidation;
using FluentValidation.Results;

namespace Common.Verifiers;

public abstract class AuthorizedMessageVerifier<TMessage, TMessageMetadata, TUnverifiedData> :
    AbstractValidator<MessageVerificationParameters<TMessage, TMessageMetadata, TUnverifiedData>>,
    IAuthorizedMessageVerifier<TMessage, TMessageMetadata, TUnverifiedData>
    where TMessage : Message
    where TMessageMetadata : MessageMetadata
{
    protected AuthorizedMessageVerifier()
    {
        RuleSet("Authorize", AuthorizationRules);
        RuleSet("Validate", ValidationRules);
    }

    protected abstract void AuthorizationRules();
    protected abstract void ValidationRules();

    public AuthorizationResult Authorize(
        MessageVerificationParameters<TMessage, TMessageMetadata, TUnverifiedData> parameters)
    {
        var authorizationResult = new AuthorizationResult();
        var result = this.Validate(parameters, options => options.IncludeRuleSets("Authorize"));

        if (!result.IsValid)
        {
            authorizationResult.AddError("Does not have authorization");
        }

        return authorizationResult;
    }

    public ValidationResult ValidateInternal(
        MessageVerificationParameters<TMessage, TMessageMetadata, TUnverifiedData> parameters)
    {
        return this.Validate(parameters, options => options.IncludeRuleSets("Validate"));
    }
}