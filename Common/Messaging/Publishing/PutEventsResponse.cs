namespace Common.Messaging.Publishing;

/// <summary>
/// Mirrors Amazon.EventBridge.Model.PutEventsResponse. <see cref="Entries"/> is in the same
/// order as the request's entries, so result i belongs to request entry i.
/// </summary>
public class PutEventsResponse
{
    public int FailedEntryCount { get; set; }
    public List<PutEventsResultEntry> Entries { get; set; } = [];
}
