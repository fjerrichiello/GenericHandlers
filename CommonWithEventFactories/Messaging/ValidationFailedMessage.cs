namespace CommonWithEventFactories.Messaging;

public abstract record ValidationFailedMessage(IDictionary<string, string[]> Errors) : Message;