namespace Common.Messaging.Publishing;

/// <summary>
/// Mirrors Amazon.EventBridge.Model.PutEventsResultEntry. A successful entry has an
/// <see cref="EventId"/>; a failed entry has <see cref="ErrorCode"/> and <see cref="ErrorMessage"/>.
/// </summary>
public class PutEventsResultEntry
{
    public string? EventId { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }

    public bool IsSuccess => string.IsNullOrEmpty(ErrorCode);
}
