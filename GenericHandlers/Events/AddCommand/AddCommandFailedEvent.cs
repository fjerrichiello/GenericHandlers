using Common.Messaging;

namespace GenericHandlers.Events.AddCommand;

public record AddCommandFailedEvent(string Reason) : Message;