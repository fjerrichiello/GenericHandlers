using CommonWithEventFactories.Messaging;

namespace CommonWithEventFactories.Events.AddCommand;

public record AddCommandValidationFailedEvent(IDictionary<string, string[]> Errors) : Message;