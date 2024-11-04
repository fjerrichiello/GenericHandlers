using Dumpify;

namespace Common.Messaging;

public class EventPublisher : IEventPublisher
{
    public async Task PublishAsync<TCommand, TEvent>(MessageContainer<TCommand, CommandMetadata> commandContainer,
        IEnumerable<TEvent> events) where TCommand : Message where TEvent : Message
    {
        var enumerable = events.ToList();
        if (enumerable.First() is AuthorizationFailedMessage)
        {
            enumerable.Select(e =>
            {
                var data = e as AuthorizationFailedMessage;
                return new
                {
                    Data = data?.Reason,
                    DetailType = typeof(TCommand).Name + "AuthorizationFailedEvent",
                    Tags = data?.Tags.Concat(["authorization"])
                };
            }).ToList().Dump();
        }

        if (enumerable.First() is ValidationFailedMessage)
        {
            enumerable.Select(e =>
            {
                var data = e as ValidationFailedMessage;
                return new
                {
                    Data = data?.Errors,
                    DetailType = typeof(TCommand).Name + "ValidationFailedEvent",
                    Tags = data?.Tags.Concat(["validation"])
                };
            }).ToList().Dump();
        }
        else
        {
            enumerable.ToList().Dump();
        }


        await Task.Delay(250);
    }

    public async Task PublishAsync<TSourceEvent, TEvent>(MessageContainer<TSourceEvent, EventMetadata> eventContainer,
        IEnumerable<TEvent> eventBodies) where TSourceEvent : Message where TEvent : Message
    {
        await Task.Delay(250);
        eventBodies.ToList().Dump();
    }
}