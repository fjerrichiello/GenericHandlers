using Common.Messaging;
using Common.Operations;
using GenericHandlers.Commands;
using GenericHandlers.Events.AddCommand;

namespace GenericHandlers.AddCommandHandler;

public class AddCommandOperation : IPublishingOperation<AddCommand, CommandMetadata, AddCommandVerifiedData,
    AddCommandFailedEvent>
{
    public async Task ExecuteAsync(MessageContainer<AddCommand, CommandMetadata> container, AddCommandVerifiedData data,
        IEventPublisher eventPublisher)
    {
        await Task.Delay(250);

        //Do Work

        await eventPublisher.PublishAsync(container, new AddedEvent(data.Value1));
    }

    public AddCommandFailedEvent CreateFailedEvent(MessageContainer<AddCommand, CommandMetadata> container, Exception e)
    {
        return new AddCommandFailedEvent(e.Message);
    }
}