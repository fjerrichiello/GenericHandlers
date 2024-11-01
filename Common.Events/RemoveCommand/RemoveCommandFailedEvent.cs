using Common.Messaging;

namespace Common.Events.RemoveCommand;

public record RemoveCommandFailedEvent(string Reason) : Message;