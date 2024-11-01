using Common.Messaging;

namespace Common.Events.SubtractCommand;

public record SubtractCommandFailedEvent(string Reason) : Message;