using Common.Messaging;

namespace GenericHandlers.Events.AddCommand;

public record AddCommandValidationFailedEvent(string Reason) : Message;