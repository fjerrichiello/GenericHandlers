using Common.Messaging;

namespace Common.Operations;

public interface IOperation<TMessage, TMessageMetadata, in TVerifiedData>
    where TMessage : Message
    where TMessageMetadata : MessageMetadata
{
    Task ExecuteAsync(MessageContainer<TMessage, TMessageMetadata> container, TVerifiedData data);
}