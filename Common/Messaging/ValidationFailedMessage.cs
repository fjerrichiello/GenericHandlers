namespace Common.Messaging;

public record ValidationFailedMessage(IDictionary<string, string[]> Errors, Type FailedEvent) : Message
{
    public List<string> Tags => EventExtensions.GetTags(FailedEvent);
}