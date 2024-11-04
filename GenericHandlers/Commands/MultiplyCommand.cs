using Common.Messaging;

namespace GenericHandlers.Commands;

public record MultiplyCommand(int Value1) : Message;