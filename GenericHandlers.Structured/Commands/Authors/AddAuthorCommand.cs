using Common.Structured.Messaging;

namespace GenericHandlers.Structured.Commands.Authors;

[FailedMessageTags("General", "Author", "Added")]
public record AddAuthorCommand(string FirstName, string LastName) : Message;