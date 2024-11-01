namespace CommonWithEventFactories.Messaging;

public interface IMessageOrchestrator
{
    Task ProcessAsync(MessageRequest request);
}