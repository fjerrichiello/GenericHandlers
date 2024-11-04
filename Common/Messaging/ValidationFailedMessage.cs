namespace Common.Messaging;

public record ValidationFailedMessage(IDictionary<string, string[]> Errors, Type FailedEvent) : Message
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