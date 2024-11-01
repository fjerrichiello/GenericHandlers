using Common.Messaging;

namespace Common.Verifiers;

public record CommandVerificationParameters<TMessage, TUnverifiedData>(
    MessageContainer<TMessage, CommandMetadata> MessageContainer,
    TUnverifiedData DataFactoryResult)
    : MessageVerificationParameters<TMessage, CommandMetadata, TUnverifiedData>(MessageContainer, DataFactoryResult)
    where TMessage : Message;