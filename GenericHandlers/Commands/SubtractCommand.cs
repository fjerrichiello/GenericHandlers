using Common.Messaging;

namespace GenericHandlers.Commands;

public record SubtractCommand(int? Value1) : Message;