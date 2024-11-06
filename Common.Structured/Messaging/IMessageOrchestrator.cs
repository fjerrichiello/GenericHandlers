namespace Common.Structured.Messaging;

public interface IMessageOrchestrator
{
    Task ProcessAsync(MessageRequest request);
}