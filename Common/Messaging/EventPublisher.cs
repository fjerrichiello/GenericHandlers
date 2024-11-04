using System.Reflection;
using System.Text.Json;
using Common.Messaging.Publishing;
using Dumpify;

namespace Common.Messaging;

public class EventPublisher : IEventPublisher
{
    public async Task PublishAsync<TCommand, TEvent>(MessageContainer<TCommand, CommandMetadata> commandContainer,
        IEnumerable<TEvent> eventBodies) where TCommand : Message where TEvent : Message
    {
        var request = new PutEventsRequest()
        {
            Entries = GetEntries(commandContainer, eventBodies)
        };

        request.Dump();
        await Task.Delay(250);
    }

    public async Task PublishAsync<TSourceEvent, TEvent>(MessageContainer<TSourceEvent, EventMetadata> eventContainer,
        IEnumerable<TEvent> eventBodies) where TSourceEvent : Message where TEvent : Message
    {
        var request = new PutEventsRequest()
        {
            Entries = GetEntries(eventContainer, eventBodies)
        };

        request.Dump();
        await Task.Delay(250);
    }

    private List<RequestEntry> GetEntries<TMessage, TEvent>(
        MessageContainer<TMessage, CommandMetadata> commandContainer,
        IEnumerable<TEvent> eventBodies)
        where TMessage : Message
        where TEvent : Message
    {
        return eventBodies.Select(
            eventBody => new RequestEntry
            {
                Source = "SourceName",
                DetailType = GetDetailType(commandContainer, eventBody),
                Detail = GetDetail(commandContainer, eventBody),
            }).ToList(); // List<PutEventsRequestEntry>
    }

    private List<RequestEntry> GetEntries<TMessage, TEvent>(
        MessageContainer<TMessage, EventMetadata> eventContainer,
        IEnumerable<TEvent> eventBodies)
        where TMessage : Message
        where TEvent : Message
    {
        return eventBodies.Select(
            eventBody => new RequestEntry
            {
                Source = "SourceName",
                DetailType = GetDetailType(eventContainer, eventBody),
                Detail = GetDetail(eventContainer, eventBody),
            }).ToList(); // List<PutEventsRequestEntry>
    }

     private string GetDetail<TMessage, TEvent>(
        MessageContainer<TMessage, CommandMetadata> commandContainer,
        TEvent eventBody)
        where TMessage : Message
        where TEvent : Message
    {
        return eventBody switch
        {
            AuthorizationFailedMessageHolder messageHolder => JsonSerializer.Serialize(new
            {
                body = messageHolder.ErrorMessage,
                tags = messageHolder.Tags
            }),
            ValidationFailedMessageHolder messageHolder => JsonSerializer.Serialize(new
            {
                body = messageHolder.ErrorMessage,
                tags = messageHolder.Tags
            }),
            _ => JsonSerializer.Serialize(new
            {
                body = eventBody,
                tags = EventExtensions.GetTags(typeof(TEvent))
            })
        };
    }

    private string GetDetail<TMessage, TEvent>(
        MessageContainer<TMessage, EventMetadata> eventContainer,
        TEvent eventBody)
        where TMessage : Message
        where TEvent : Message
    {
        return eventBody switch
        {
            AuthorizationFailedMessageHolder messageHolder => JsonSerializer.Serialize(new
            {
                body = messageHolder.ErrorMessage,
                tags = messageHolder.Tags
            }),
            ValidationFailedMessageHolder messageHolder => JsonSerializer.Serialize(new
            {
                body = messageHolder.ErrorMessage,
                tags = messageHolder.Tags
            }),
            _ => JsonSerializer.Serialize(new
            {
                body = eventBody,
                tags = EventExtensions.GetTags(typeof(TEvent))
            })
        };
    }

    private static string GetDetailType<TMessage, TMessageMetadata, TEvent>(
        MessageContainer<TMessage, TMessageMetadata> messageContainer,
        TEvent eventBody)
        where TMessage : Message
        where TMessageMetadata : MessageMetadata
        where TEvent : Message
        => eventBody switch
        {
            AuthorizationFailedMessageHolder => typeof(TMessage).Name + "AuthorizationFailed",
            ValidationFailedMessageHolder => typeof(TMessage).Name + "ValidationFailed",
            _ => typeof(TEvent).Name
        };
}