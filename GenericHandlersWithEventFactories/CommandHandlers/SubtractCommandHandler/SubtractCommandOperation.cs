﻿using CommonWithEventFactories.Events.SubtractCommand;
using CommonWithEventFactories.Messaging;
using CommonWithEventFactories.Operations;
using GenericHandlersWithEventFactories.Commands;

namespace GenericHandlersWithEventFactories.CommandHandlers.SubtractCommandHandler;

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