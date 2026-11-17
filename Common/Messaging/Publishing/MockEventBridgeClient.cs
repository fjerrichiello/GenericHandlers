using Dumpify;

namespace Common.Messaging.Publishing;

/// <summary>
/// Stand-in for the AWS EventBridge client. Returns one result entry per request entry, in order,
/// the way PutEvents does.
/// </summary>
public class MockEventBridgeClient : IEventBridgeClient
{
    // PutEvents accepts at most 10 entries per call.
    public const int MaxEntriesPerRequest = 10;

    public async Task<PutEventsResponse> PutEventsAsync(PutEventsRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.Entries is null || request.Entries.Count is 0)
        {
            throw new ArgumentException("PutEventsRequest must contain at least one entry.", nameof(request));
        }

        if (request.Entries.Count > MaxEntriesPerRequest)
        {
            throw new ArgumentException(
                $"PutEventsRequest may contain at most {MaxEntriesPerRequest} entries; got {request.Entries.Count}.",
                nameof(request));
        }

        request.Dump();
        await Task.Delay(250, cancellationToken);

        var entries = request.Entries.Select(ToResultEntry).ToList();

        var response = new PutEventsResponse
        {
            Entries = entries,
            FailedEntryCount = entries.Count(entry => !entry.IsSuccess)
        };

        response.Dump();
        return response;
    }

    private static PutEventsResultEntry ToResultEntry(RequestEntry entry)
    {
        if (string.IsNullOrWhiteSpace(entry.Source) ||
            string.IsNullOrWhiteSpace(entry.DetailType) ||
            string.IsNullOrWhiteSpace(entry.Detail))
        {
            return new PutEventsResultEntry
            {
                ErrorCode = "InvalidArgument",
                ErrorMessage = "Source, DetailType and Detail are required."
            };
        }

        return new PutEventsResultEntry { EventId = Guid.NewGuid().ToString() };
    }
}
