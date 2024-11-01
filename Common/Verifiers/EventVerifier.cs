using Common.Messaging;
using FluentValidation;
using FluentValidation.Results;

namespace Common.Verifiers;

public abstract class EventVerifier<TMessage, TUnverifiedData> :
    AbstractValidator<MessageVerificationParameters<TMessage, EventMetadata, TUnverifiedData>>,
    IEventVerifier<TMessage, TUnverifiedData>
    where TMessage : Message
{
    protected EventVerifier()
    {
        RuleSet("Validate", ValidationRules);
    }

    protected abstract void ValidationRules();

    ValidationResult IEventVerifier<TMessage, TUnverifiedData>.Validate(
        MessageVerificationParameters<TMessage, EventMetadata, TUnverifiedData> parameters)
    {
        return this.Validate(parameters, options => options.IncludeRuleSets("Validate"));
    }
}