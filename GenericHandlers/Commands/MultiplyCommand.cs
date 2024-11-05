using Common.Messaging;

namespace GenericHandlers.Commands;

[FailedMessageTags("BoundedContext", "EntityType", "Multiply")]
public record MultiplyCommand(int Value1) : Message;