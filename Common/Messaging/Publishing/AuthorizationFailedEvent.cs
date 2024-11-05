namespace Common.Messaging.Publishing;

public sealed record AuthorizationFailedEvent(string Reason) : Message;