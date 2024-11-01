using CommonWithEventFactories.Messaging;

namespace CommonWithEventFactories.Events.RemoveCommand;

public record RemovedEvent(int Value) : Message;