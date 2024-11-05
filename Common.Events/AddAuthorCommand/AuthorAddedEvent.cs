using Common.Messaging;

namespace Common.Events.AddAuthorCommand;

[MessageTags("General", "Author", "Added", "Success")]
public record AuthorAddedEvent(int AuthorId, string FirstName, string LastName) : Message;