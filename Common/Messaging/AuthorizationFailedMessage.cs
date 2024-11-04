namespace Common.Messaging;

public record AuthorizationFailedMessage(string Reason, Type FailedEvent) : Message
{
    public List<string> Tags => EventExtensions.GetTags(FailedEvent);
}