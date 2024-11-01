using CommonWithEventFactories.Messaging;
using FluentValidation;
using FluentValidation.Results;

namespace CommonWithEventFactories.Verifiers;

public abstract class MessageVerifier<TMessage, TMessageMetadata, TUnverifiedData> :
    AbstractValidator<MessageVerificationParameters<TMessage, TMessageMetadata, TUnverifiedData>>,
    IMessageVerifier<TMessage, TMessageMetadata, TUnverifiedData>
    where TMessage : Message
    where TMessageMetadata : MessageMetadata
{
    protected MessageVerifier()
    {
        RuleSet("Validate", ValidationRules);
    }

    protected abstract void ValidationRules();

    ValidationResult IMessageVerifier<TMessage, TMessageMetadata, TUnverifiedData>.Validate(
        MessageVerificationParameters<TMessage, TMessageMetadata, TUnverifiedData> parameters)
    {
        return this.Validate(parameters, options => options.IncludeRuleSets("Validate"));
    }
}