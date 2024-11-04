namespace Common.Messaging;

public record AuthorizationFailedMessageHolder : Message
{
    public AuthorizationFailedMessageHolder(string reason, Type failedEventType)
    {
        Tags = EventExtensions.GetGenericFailedTags(failedEventType).Concat(
            EventExtensions.GetTags(typeof(AuthorizationFailedMessage))).Distinct().ToList();
        ErrorMessage = new AuthorizationFailedMessage(reason);
    }

    public List<string> Tags { get; }

    public AuthorizationFailedMessage ErrorMessage { get; }
}