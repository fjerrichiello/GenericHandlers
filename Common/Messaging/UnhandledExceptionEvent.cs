namespace Common.Messaging;

public record UnhandledExceptionEvent(string Reason) : Message;