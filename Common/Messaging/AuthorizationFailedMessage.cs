namespace Common.Messaging;

[MessageTags("Authorization-Failed")]
public record AuthorizationFailedMessage(string Reason) : Message;