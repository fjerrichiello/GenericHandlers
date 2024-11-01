using CommonWithEventFactories.Messaging;

namespace CommonWithEventFactories.Verifiers;

public record MessageVerificationParameters<TMessage, TMessageMetadata, TUnverifiedData>(
    MessageContainer<TMessage, TMessageMetadata> MessageContainer,
    TUnverifiedData DataFactoryResult)
    where TMessage : Message
    where TMessageMetadata : MessageMetadata;