using Common.Messaging;

namespace GenericHandlers.Events.AddCommand;

public record AddCommandValidationFailedEvent(IDictionary<string, string[]> Errors) : Message;