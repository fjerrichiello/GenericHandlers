using Common.Events.MultiplyCommand;
using Common.Messaging;
using Common.Operations;
using GenericHandlers.Commands;

namespace GenericHandlers.CommandHandlers.MultiplyCommandHandler;

public class MultiplyCommandOperation(IEventPublisher _eventPublisher) : IOperation<MultiplyCommand, CommandMetadata,
    MultiplyCommandVerifiedData>
{
    public async Task ExecuteAsync(MessageContainer<MultiplyCommand, CommandMetadata> container,
        MultiplyCommandVerifiedData data)
    {
        await Task.Delay(250);

        //Do Work

        await _eventPublisher.PublishAsync(container, new MultipliedEvent(data.Value1));
    }
}