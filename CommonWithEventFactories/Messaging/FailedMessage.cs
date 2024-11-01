namespace CommonWithEventFactories.Messaging;

public abstract record FailedMessage(string Reason) : Message;