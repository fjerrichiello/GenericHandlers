namespace Common.Messaging;

[MessageTags("Validation-Failed")]
public record ValidationFailedMessage(IDictionary<string, string[]> Errors) : Message;