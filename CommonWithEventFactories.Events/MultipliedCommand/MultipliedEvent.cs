using CommonWithEventFactories.Messaging;

namespace CommonWithEventFactories.Events.MultipliedCommand;

public record MultipliedEvent(int Value) : Message;