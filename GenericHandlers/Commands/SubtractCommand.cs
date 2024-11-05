using Common.Messaging;

namespace GenericHandlers.Commands;

[FailedMessageTags("BoundedContext", "EntityType", "Subtract")]
public record SubtractCommand(int? Value1) : Message;