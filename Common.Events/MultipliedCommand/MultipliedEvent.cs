using Common.Messaging;

namespace Common.Events.MultipliedCommand;

public record MultipliedEvent(int Value) : Message;