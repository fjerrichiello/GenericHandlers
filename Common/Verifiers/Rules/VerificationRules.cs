using Common.Messaging;
using Common.Models.Authors;

namespace Common.Verifiers.Rules;

public static class VerificationRules
{
    public static bool HasFirstName<TMessage, TMessageMetadata, TDataFactoryResult>(
        MessageVerificationParameters<TMessage, TMessageMetadata, TDataFactoryResult> messageVerificationParameters)
        where TMessage : Message
        where TMessageMetadata : MessageMetadata
        where TDataFactoryResult : IAuthorCommandData<IAuthorLike>
    {
        return !string.IsNullOrWhiteSpace(messageVerificationParameters.DataFactoryResult.Author?.FirstName);
    }
}