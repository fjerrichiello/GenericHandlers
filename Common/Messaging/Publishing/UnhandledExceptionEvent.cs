namespace Common.Messaging.Publishing;

public sealed record UnhandledExceptionEvent(string Reason) : Message;