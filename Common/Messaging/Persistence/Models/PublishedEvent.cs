namespace Common.Messaging.Persistence.Models;

public class PublishedEvent : MessageEntity
{
    public Guid CommandId { get; init; }
    
    public string CommandType { get; init; } = string.Empty;
}