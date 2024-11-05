namespace Common.Messaging;

public record ValidationFailedEvent(IDictionary<string, string[]> Errors) : Message;