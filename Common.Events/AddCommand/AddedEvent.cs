using Common.Messaging;

namespace Common.Events.AddCommand;

public record AddedEvent(int Value) : Message;