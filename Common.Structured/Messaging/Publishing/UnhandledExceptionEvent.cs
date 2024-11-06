namespace Common.Structured.Messaging.Publishing;

public sealed record UnhandledExceptionEvent(string Reason) : Message;