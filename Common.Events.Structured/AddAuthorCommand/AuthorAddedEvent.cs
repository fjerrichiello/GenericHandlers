using Common.Structured.Messaging;

namespace Common.Events.Structured.AddAuthorCommand;

[MessageTags("General", "Author", "Added", "Success")]
public record AuthorAddedEvent(int AuthorId, string FirstName, string LastName) : Message;