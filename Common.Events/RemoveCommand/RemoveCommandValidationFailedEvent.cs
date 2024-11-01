using Common.Messaging;

namespace Common.Events.RemoveCommand;

public record RemoveCommandValidationFailedEvent(IDictionary<string, string[]> Errors) : Message;