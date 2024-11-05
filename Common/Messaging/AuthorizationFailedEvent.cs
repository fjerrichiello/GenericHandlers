namespace Common.Messaging;

public record AuthorizationFailedEvent(string Reason) : Message;