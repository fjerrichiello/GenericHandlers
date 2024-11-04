namespace Common.Messaging;

public record AuthorizationFailedMessage(string Reason) : Message;