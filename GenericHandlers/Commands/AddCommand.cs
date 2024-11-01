using Common;
using Common.Messaging;

namespace GenericHandlers.Commands;

public record AddCommand(int? Value1) : Message;