using Common.Messaging;

namespace Common.Events.MultiplyCommand;

[MessageTags("Error1", "Error2", "Error3", "Failed")]
public record MultiplyCommandFailedEvent(string Reason) : Message;