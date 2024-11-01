using Common.Messaging;

namespace Common.Services;

public interface IHandlerService<TMessage, TMessageMetadata, in TVerifiedData, out TFailedEvent, TSuccessEvent>
    where TMessage : Message
    where TMessageMetadata : MessageMetadata
{
    Task<TSuccessEvent> ProcessAsync(MessageContainer<TMessage, TMessageMetadata> container, TVerifiedData data);

    TFailedEvent CreateFailedEvent(MessageContainer<TMessage, TMessageMetadata> container, Exception e);
}