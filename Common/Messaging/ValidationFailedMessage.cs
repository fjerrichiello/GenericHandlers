namespace Common.Messaging;

public abstract record ValidationFailedMessage(IDictionary<string, string[]> Errors) : Message;