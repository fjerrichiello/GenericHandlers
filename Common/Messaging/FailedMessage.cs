namespace Common.Messaging;

public record FailedMessage(string Reason) : Message;