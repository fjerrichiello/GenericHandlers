using Common.Messaging;

namespace Common.Operations;

public interface IEventOperation<TMessage, in TVerifiedData>
    where TMessage : Message
{
    Task ExecuteAsync(MessageContainer<TMessage, EventMetadata> container, TVerifiedData data);
}