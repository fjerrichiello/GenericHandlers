using Common.Messaging;

namespace GenericHandlers.Commands;

[FailedMessageTags("BoundedContext", "EntityType", "Add")]
public record AddCommand(int? Value1) : Message;