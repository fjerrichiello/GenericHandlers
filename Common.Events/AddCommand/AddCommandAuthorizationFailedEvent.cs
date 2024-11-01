using Common.Messaging;

namespace Common.Events.AddCommand;

public record AddCommandAuthorizationFailedEvent(string Reason) : Message;