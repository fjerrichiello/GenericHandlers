namespace CommonWithEventFactories.Messaging;

public abstract record AuthorizationFailedMessage(string Reason) : FailedMessage(Reason);