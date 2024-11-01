namespace Common.Messaging;

public interface IMessageOrchestrator
{
    Task ProcessAsync(MessageRequest request);
}