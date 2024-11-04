using Common.Messaging;

namespace Common.Events.DividedCommand;

public record DividedEvent(int Value) : Message;