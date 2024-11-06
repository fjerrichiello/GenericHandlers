using Common.Structured.Messaging;

namespace GenericHandlers.StructuredCommands.Authors;

[FailedMessageTags("General", "Author", "Added")]
public record AddAuthorCommand(string FirstName, string LastName) : Message;