using CommonWithEventFactories.Messaging;

namespace CommonWithEventFactories.Events.SubtractCommand;

public record SubtractCommandValidationFailedEvent(IDictionary<string, string[]> Errors) : Message;