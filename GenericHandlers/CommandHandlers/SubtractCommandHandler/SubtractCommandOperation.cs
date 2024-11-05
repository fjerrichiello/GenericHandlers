using Common.Events.SubtractCommand;
using Common.Messaging;
using Common.Operations;
using GenericHandlers.Commands;

namespace GenericHandlers.CommandHandlers.SubtractCommandHandler;

public class SubtractCommandOperation(IEventPublisher _eventPublisher)
    : IOperation<SubtractCommand, CommandMetadata, SubtractCommandVerifiedData>
{
    public async Task ExecuteAsync(MessageContainer<SubtractCommand, CommandMetadata> container,
        SubtractCommandVerifiedData data)
    {
        await Task.Delay(250);

        //Do Work

        await _eventPublisher.PublishAsync(container, new SubtractedEvent(data.Value1));
    }
}