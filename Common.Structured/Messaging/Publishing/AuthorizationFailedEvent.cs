namespace Common.Structured.Messaging.Publishing;

public sealed record AuthorizationFailedEvent(string Reason) : Message;