using CommonWithEventFactories.Messaging;

namespace CommonWithEventFactories.Events.RemoveCommand;

public record RemoveCommandValidationFailedEvent(IDictionary<string, string[]> Errors) : Message;