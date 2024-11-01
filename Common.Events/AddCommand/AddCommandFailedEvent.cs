using Common.Messaging;

namespace Common.Events.AddCommand;

public record AddCommandFailedEvent(string Reason) : Message;