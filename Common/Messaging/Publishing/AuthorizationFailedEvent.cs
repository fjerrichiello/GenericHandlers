namespace Common.Messaging.Publishing;

public record AuthorizationFailedEvent(string Reason) : Message;