using Common.Messaging;

namespace Common.Events.RemoveCommand;

public record RemovedEvent(int Value) : Message;