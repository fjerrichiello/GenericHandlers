namespace Common.Messaging;

public interface IMessageContainerHandler<TMessage, TMessageMetadata>
    where TMessage : Message
    where TMessageMetadata : MessageMetadata
{
    Task HandleAsync(
        MessageContainer<TMessage, TMessageMetadata> container);
}