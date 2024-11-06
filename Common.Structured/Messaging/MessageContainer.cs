namespace Common.Structured.Messaging;

public record MessageContainer<TMessage, TMessageMetadata>(
    TMessage Message,
    TMessageMetadata Metadata)
    where TMessage : Message
    where TMessageMetadata : MessageMetadata;