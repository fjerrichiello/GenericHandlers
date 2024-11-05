using Common.Events.AddCommand;
using Common.Messaging;
using Common.Operations;
using GenericHandlers.Commands;

namespace GenericHandlers.CommandHandlers.AddCommandHandler;

public class AddCommandOperation(IEventPublisher _eventPublisher)
    : IOperation<AddCommand, CommandMetadata, AddCommandVerifiedData>
{
    public async Task ExecuteAsync(MessageContainer<AddCommand, CommandMetadata> container, AddCommandVerifiedData data)
    {
        await Task.Delay(250);

        //Do Work

        await _eventPublisher.PublishAsync(container, new AddedEvent(data.Value1));
    }
}