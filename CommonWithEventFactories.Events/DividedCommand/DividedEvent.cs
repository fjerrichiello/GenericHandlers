using CommonWithEventFactories.Messaging;

namespace CommonWithEventFactories.Events.DividedCommand;

public record DividedEvent(int Value) : Message;