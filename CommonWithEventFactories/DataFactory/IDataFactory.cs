using CommonWithEventFactories.Messaging;

namespace CommonWithEventFactories.DataFactory;

public interface IDataFactory<TMessage, TMessageMetadata, TUnverifiedData, out TVerifiedData>
    where TMessage : Message
    where TMessageMetadata : MessageMetadata
{
    Task<TUnverifiedData> GetDataAsync(MessageContainer<TMessage, TMessageMetadata> container);

    TVerifiedData GetVerifiedData(TUnverifiedData unverifiedData);
}