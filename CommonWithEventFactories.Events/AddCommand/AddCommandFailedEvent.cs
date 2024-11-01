using CommonWithEventFactories.Messaging;

namespace CommonWithEventFactories.Events.AddCommand;

public record AddCommandFailedEvent(string Reason) : Message;