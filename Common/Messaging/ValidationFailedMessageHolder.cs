namespace Common.Messaging;

public record ValidationFailedMessageHolder : Message
{
    public ValidationFailedMessageHolder(IDictionary<string, string[]> errors, Type failedEventType)
    {
        Tags = EventExtensions.GetGenericFailedTags(failedEventType).Concat(
            EventExtensions.GetTags(typeof(ValidationFailedMessage))).Distinct().ToList();
        ErrorMessage = new ValidationFailedMessage(errors);
    }

    public List<string> Tags { get; }

    public ValidationFailedMessage ErrorMessage { get; }
}