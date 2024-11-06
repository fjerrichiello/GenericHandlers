using Common.Messaging;

namespace Common.DataFactory;

public interface IDataFactory<TMessage, TMessageMetadata, TUnverifiedData, out TVerifiedData>
    where TMessage : Message
    where TMessageMetadata : MessageMetadata
{
    Task<TUnverifiedData> GetUnverifiedDataAsync(MessageContainer<TMessage, TMessageMetadata> container);

    TVerifiedData GetVerifiedData(TUnverifiedData unverifiedData);
}