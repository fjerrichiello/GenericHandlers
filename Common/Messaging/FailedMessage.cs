namespace Common.Messaging;

public abstract record FailedMessage(string Reason) : Message;