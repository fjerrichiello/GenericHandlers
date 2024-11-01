using Common.Messaging;

namespace GenericHandlers.Events.AddCommand;

public record AddedEvent(int Value) : Message;