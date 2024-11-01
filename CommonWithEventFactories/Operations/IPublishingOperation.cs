using CommonWithEventFactories.Messaging;

namespace CommonWithEventFactories.Operations;

public interface IPublishingOperation<TMessage, TMessageMetadata, in TVerifiedData, out TFailedEvent>
    where TMessage : Message
    where TMessageMetadata : MessageMetadata
    where TFailedEvent : Message
{
    Task ExecuteAsync(MessageContainer<TMessage, TMessageMetadata> container, TVerifiedData data,
        IEventPublisher eventPublisher);

    TFailedEvent CreateFailedEvent(MessageContainer<TMessage, TMessageMetadata> container, Exception e);
}