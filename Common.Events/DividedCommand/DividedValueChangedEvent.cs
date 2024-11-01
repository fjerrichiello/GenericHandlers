using Common.Messaging;

namespace Common.Events.DividedCommand;

public record DividedValueChangedEvent(int Value) : Message;