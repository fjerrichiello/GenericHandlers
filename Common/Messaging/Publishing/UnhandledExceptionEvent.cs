namespace Common.Messaging.Publishing;

public record UnhandledExceptionEvent(string Reason) : Message;