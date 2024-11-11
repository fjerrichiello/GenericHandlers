using Common.Messaging;
using FluentValidation;
using FluentValidation.Results;

namespace Common.Verifiers;

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

    public ValidationResult ValidateInternal(
        MessageVerificationParameters<TMessage, TMessageMetadata, TUnverifiedData> parameters)
    {
        return this.Validate(parameters, options => options.IncludeRuleSets("Validate"));
    }
}