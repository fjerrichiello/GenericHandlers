using Common.Messaging;

namespace Common.Events.MultiplyCommand;

[MessageTags("Success")]
public record MultipliedEvent(int Value) : Message;