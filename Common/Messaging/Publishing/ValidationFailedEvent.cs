namespace Common.Messaging.Publishing;

public record ValidationFailedEvent(IDictionary<string, string[]> Errors) : Message;