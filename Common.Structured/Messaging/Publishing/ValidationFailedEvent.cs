namespace Common.Structured.Messaging.Publishing;

public sealed record ValidationFailedEvent(IDictionary<string, string[]> Errors) : Message;