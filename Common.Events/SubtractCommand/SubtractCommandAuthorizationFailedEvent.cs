using Common.Messaging;

namespace Common.Events.SubtractCommand;

public record SubtractCommandAuthorizationFailedEvent(string Reason) : Message;