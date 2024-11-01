using Common.Messaging;

namespace Common.Events.AddCommand;

public record AddCommandValidationFailedEvent(IDictionary<string, string[]> Errors) : Message;