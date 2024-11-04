using Common.Messaging;

namespace Common.Events.AddCommand;

[MessageTags("Error1", "Error2", "Error3", "Failed")]
public record AddCommandFailedEvent(string Reason) : Message;