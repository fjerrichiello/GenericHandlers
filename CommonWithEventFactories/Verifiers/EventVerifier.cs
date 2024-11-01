
using CommonWithEventFactories.Messaging;
using FluentValidation;
using FluentValidation.Results;

namespace CommonWithEventFactories.Verifiers;

public abstract class EventVerifier<TEvent, TUnverifiedData> :
    AbstractValidator<MessageVerificationParameters<TEvent, EventMetadata, TUnverifiedData>>,
    IEventVerifier<TEvent, TUnverifiedData>
    where TEvent : Message
{
    protected EventVerifier()
    {
        RuleSet("Validate", ValidationRules);
    }

    protected abstract void ValidationRules();

    ValidationResult IEventVerifier<TEvent, TUnverifiedData>.Validate(
        MessageVerificationParameters<TEvent, EventMetadata, TUnverifiedData> parameters)
    {
        return this.Validate(parameters, options => options.IncludeRuleSets("Validate"));
    }
}