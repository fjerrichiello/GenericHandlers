using Common.Messaging;

namespace Common.Operations;

public interface IEventPublishingOperation<TMessage, in TVerifiedData>
    where TMessage : Message
{
    Task ExecuteAsync(MessageContainer<TMessage, EventMetadata> container, TVerifiedData data,
        IEventPublisher eventPublisher);
}