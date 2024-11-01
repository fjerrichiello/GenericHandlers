using Common.Messaging;
using FluentValidation;
using FluentValidation.Results;

namespace Common.Verifiers;

public abstract class MessageVerifier<TMessage, TMessageMetadata, TUnverifiedData,
    TValidationFailedEvent> :
    AbstractValidator<MessageVerificationParameters<TMessage, TMessageMetadata, TUnverifiedData>>,
    IMessageVerifier<TMessage, TMessageMetadata, TUnverifiedData, TValidationFailedEvent>
    where TMessage : Message
    where TMessageMetadata : MessageMetadata
    where TValidationFailedEvent : Message
{
    protected MessageVerifier()
    {
        RuleSet("Validate", ValidationRules);
    }

    protected abstract void ValidationRules();

    ValidationResult IMessageVerifier<TMessage, TMessageMetadata, TUnverifiedData, TValidationFailedEvent>.Validate(
        MessageVerificationParameters<TMessage, TMessageMetadata, TUnverifiedData> parameters)
    {
        return this.Validate(parameters, options => options.IncludeRuleSets("Validate"));
    }

    public abstract TValidationFailedEvent CreateValidationFailedEvent(
        MessageVerificationParameters<TMessage, TMessageMetadata, TUnverifiedData> parameters,
        ValidationResult validationResult);
}