namespace Common.Messaging;

public record AuthorizationFailedMessage(string Reason, Type FailedEvent) : Message
{
    public List<string> Tags
    {
        get
        {
            var tags = EventExtensions.GetTags(FailedEvent);
            tags.Remove("failed");
            return tags;
        }
    }
}