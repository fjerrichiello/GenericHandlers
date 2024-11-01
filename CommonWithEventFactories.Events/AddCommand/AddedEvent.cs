using CommonWithEventFactories.Messaging;

namespace CommonWithEventFactories.Events.AddCommand;

public record AddedEvent(int Value) : Message;