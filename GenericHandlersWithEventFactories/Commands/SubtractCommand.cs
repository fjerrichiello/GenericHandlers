using CommonWithEventFactories.Messaging;

namespace GenericHandlersWithEventFactories.Commands;

public record SubtractCommand(int? Value1) : Message;