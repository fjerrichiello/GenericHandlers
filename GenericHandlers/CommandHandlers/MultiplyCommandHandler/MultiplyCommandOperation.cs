using Common.Events.MultiplyCommand;
using Common.Messaging;
using Common.Operations;
using GenericHandlers.Commands;

namespace GenericHandlers.CommandHandlers.MultiplyCommandHandler;

public class MultiplyCommandOperation : IPublishingOperation<MultiplyCommand, CommandMetadata,
    MultiplyCommandVerifiedData,
    MultiplyCommandFailedEvent>
{
    public async Task ExecuteAsync(MessageContainer<MultiplyCommand, CommandMetadata> container,
        MultiplyCommandVerifiedData data,
        IEventPublisher eventPublisher)
    {
        await Task.Delay(250);

        //Do Work

        await eventPublisher.PublishAsync(container, new MultipliedEvent(data.Value1));
    }

    public MultiplyCommandFailedEvent CreateFailedEvent(MessageContainer<MultiplyCommand, CommandMetadata> container,
        Exception e)
    {
        return new MultiplyCommandFailedEvent(e.Message);
    }
}