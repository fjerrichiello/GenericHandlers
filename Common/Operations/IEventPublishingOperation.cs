using Common.Messaging;

namespace Common.Operations;

public interface IEventPublishingOperation<TMessage, TMessageMetadata, in TVerifiedData>
    where TMessage : Message
    where TMessageMetadata : MessageMetadata
{
    Task ExecuteAsync(MessageContainer<TMessage, TMessageMetadata> container, TVerifiedData data,
        IEventPublisher eventPublisher);
}