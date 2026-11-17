using System.Text.Json;

namespace Common.Messaging.Persistence.Models;

public abstract class MessageEntity
{
    public Guid Id { get; init; }

    public DateTimeOffset Timestamp { get; init; }

    public DateTimeOffset SystemTimestamp { get; init; }

    public string MessageType { get; init; } = string.Empty;

    public JsonElement Payload { get; init; }
}