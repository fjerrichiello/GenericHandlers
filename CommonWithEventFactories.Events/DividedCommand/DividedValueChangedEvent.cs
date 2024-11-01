using CommonWithEventFactories.Messaging;

namespace CommonWithEventFactories.Events.DividedCommand;

public record DividedValueChangedEvent(int Value) : Message;