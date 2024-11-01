using CommonWithEventFactories.Messaging;

namespace GenericHandlersWithEventFactories.Commands;

public record AddCommand(int? Value1) : Message;