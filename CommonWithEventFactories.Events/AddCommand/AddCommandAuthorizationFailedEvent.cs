using CommonWithEventFactories.Messaging;

namespace CommonWithEventFactories.Events.AddCommand;

public record AddCommandAuthorizationFailedEvent(string Reason) : Message;