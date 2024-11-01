using Common.Events.SubtractCommand;
using Common.Messaging;
using Common.Operations;
using GenericHandlers.Commands;

namespace GenericHandlers.CommandHandlers.SubtractCommandHandler;

public class SubtractCommandOperation : IPublishingOperation<SubtractCommand, CommandMetadata, SubtractCommandVerifiedData,
    SubtractCommandFailedEvent>
{
    public async Task ExecuteAsync(MessageContainer<SubtractCommand, CommandMetadata> container, SubtractCommandVerifiedData data,
        IEventPublisher eventPublisher)
    {
        await Task.Delay(250);

        //Do Work

        await eventPublisher.PublishAsync(container, new SubtractedEvent(data.Value1));
    }

    public SubtractCommandFailedEvent CreateFailedEvent(MessageContainer<SubtractCommand, CommandMetadata> container, Exception e)
    {
        return new SubtractCommandFailedEvent(e.Message);
    }
}