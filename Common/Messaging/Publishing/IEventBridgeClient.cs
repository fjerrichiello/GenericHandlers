namespace Common.Messaging.Publishing;

/// <summary>
/// The subset of Amazon.EventBridge.IAmazonEventBridge the publisher uses, so the mock can be
/// swapped for the real client later.
/// </summary>
public interface IEventBridgeClient
{
    Task<PutEventsResponse> PutEventsAsync(PutEventsRequest request, CancellationToken cancellationToken = default);
}
