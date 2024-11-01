using CommonWithEventFactories.Messaging;

namespace CommonWithEventFactories.Events.RemoveCommand;

public record RemoveCommandFailedEvent(string Reason) : Message;