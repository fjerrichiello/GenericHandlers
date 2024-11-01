using Common.Messaging;

namespace Common.Events.SubtractCommand;

public record SubtractCommandValidationFailedEvent(IDictionary<string, string[]> Errors) : Message;