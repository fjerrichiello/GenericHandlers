using Common.Messaging;

namespace GenericHandlers.Commands.Authors;

[FailedMessageTags("General", "Author", "Added")]
public record AddAuthorCommand(string FirstName, string LastName) : Message;