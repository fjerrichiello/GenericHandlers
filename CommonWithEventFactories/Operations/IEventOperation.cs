using CommonWithEventFactories.Messaging;

namespace CommonWithEventFactories.Operations;

public interface IEventOperation<TMessage, TMessageMetadata, in TVerifiedData>
    where TMessage : Message
    where TMessageMetadata : MessageMetadata
{
    Task ExecuteAsync(MessageContainer<TMessage, TMessageMetadata> container, TVerifiedData data);
}