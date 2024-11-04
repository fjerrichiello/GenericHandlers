namespace Common.Messaging;

public record ValidationFailedMessage(IDictionary<string, string[]> Errors) : Message;