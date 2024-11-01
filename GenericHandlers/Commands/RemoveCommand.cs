using Common.Messaging;

namespace GenericHandlers.Commands;

public record RemoveCommand(int? Value1) : Message;