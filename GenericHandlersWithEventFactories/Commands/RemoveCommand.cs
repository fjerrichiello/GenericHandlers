using CommonWithEventFactories.Messaging;

namespace GenericHandlersWithEventFactories.Commands;

public record RemoveCommand(int? Value1) : Message;