using Common.Events.RemoveCommand;
using Common.Messaging;
using Common.Operations;
using GenericHandlers.Commands;

namespace GenericHandlers.CommandHandlers.RemoveCommandHandler;

public class RemoveCommandOperation : IPublishingOperation<RemoveCommand, CommandMetadata, RemoveCommandVerifiedData,
    RemoveCommandFailedEvent>
{
    public async Task ExecuteAsync(MessageContainer<RemoveCommand, CommandMetadata> container,
        RemoveCommandVerifiedData data,
        IEventPublisher eventPublisher)
    {
        await Task.Delay(250);

        //Do Work

        await eventPublisher.PublishAsync(container, new RemovedEvent(data.Value1));
    }

    public RemoveCommandFailedEvent CreateFailedEvent(MessageContainer<RemoveCommand, CommandMetadata> container,
        Exception e)
    {
        return new RemoveCommandFailedEvent(e.Message);
    }
}