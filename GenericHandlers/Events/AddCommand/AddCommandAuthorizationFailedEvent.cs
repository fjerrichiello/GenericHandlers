using Common.Messaging;

namespace GenericHandlers.Events.AddCommand;

public record AddCommandAuthorizationFailedEvent(string Reason) : Message;