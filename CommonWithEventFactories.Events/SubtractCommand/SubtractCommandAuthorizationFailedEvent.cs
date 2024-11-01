using CommonWithEventFactories.Messaging;

namespace CommonWithEventFactories.Events.SubtractCommand;

public record SubtractCommandAuthorizationFailedEvent(string Reason) : Message;