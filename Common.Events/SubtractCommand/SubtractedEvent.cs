using Common.Messaging;

namespace Common.Events.SubtractCommand;

public record SubtractedEvent(int Value) : Message;