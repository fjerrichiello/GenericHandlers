using Common.Messaging;

namespace Common.Verifiers;

public interface
    IAuthorizedMessageVerifier<TMessage, TMessageMetadata, TUnverifiedData> : IMessageVerifier<TMessage,
    TMessageMetadata,
    TUnverifiedData>
    where TMessage : Message
    where TMessageMetadata : MessageMetadata
{
    AuthorizationResult Authorize(
        MessageVerificationParameters<TMessage, TMessageMetadata, TUnverifiedData> parameters);
}