using CommonWithEventFactories.Messaging;

namespace CommonWithEventFactories.Events.SubtractCommand;

public record SubtractCommandFailedEvent(string Reason) : Message;