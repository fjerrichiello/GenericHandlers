namespace Common.Messaging;

public abstract record AuthorizationFailedMessage(string Reason) : FailedMessage(Reason);