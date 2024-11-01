using CommonWithEventFactories.Messaging;

namespace CommonWithEventFactories.Events.SubtractCommand;

public record SubtractedEvent(int Value) : Message;