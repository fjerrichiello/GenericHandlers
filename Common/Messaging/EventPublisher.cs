using System.Reflection;
using System.Text.Json;
using Common.Messaging.Publishing;
using Common.Messaging.Persistence;
using Common.Messaging.Persistence.Models;
using Common.Transactions;
using Microsoft.Extensions.Logging;

namespace Common.Messaging;

public class EventPublisher(
    IEventBridgeClient _eventBridgeClient,
    ITransaction<PublishedEventRecorder> _transaction,
    ILogger<EventPublisher> _logger) : IEventPublisher
{
    public async Task PublishAsync<TCommand, TEvent>(MessageContainer<TCommand, CommandMetadata> commandContainer,
        IEnumerable<TEvent> eventBodies) where TCommand : Message where TEvent : Message
    {
        await PublishEntriesAsync(commandContainer, GetEntries(commandContainer, eventBodies));
    }

    public async Task PublishAsync<TSourceEvent, TEvent>(MessageContainer<TSourceEvent, EventMetadata> eventContainer,
        IEnumerable<TEvent> eventBodies) where TSourceEvent : Message where TEvent : Message
    {
        await PublishEntriesAsync(eventContainer, GetEntries(eventContainer, eventBodies));
    }

    public async Task PublishAuthorizationFailedAsync<TCommand>(
        MessageContainer<TCommand, CommandMetadata> commandContainer, AuthorizationFailedEvent eventBody)
        where TCommand : Message
    {
        await PublishEntriesAsync(commandContainer, [GetEntry(commandContainer, eventBody)]);
    }

    public async Task PublishValidationFailedAsync<TCommand>(
        MessageContainer<TCommand, CommandMetadata> commandContainer, ValidationFailedEvent eventBody)
        where TCommand : Message
    {
        await PublishEntriesAsync(commandContainer, [GetEntry(commandContainer, eventBody)]);
    }

    public async Task PublishValidationFailedAsync<TSourceEvent>(
        MessageContainer<TSourceEvent, EventMetadata> eventContainer, ValidationFailedEvent eventBody)
        where TSourceEvent : Message
    {
        await PublishEntriesAsync(eventContainer, [GetEntry(eventContainer, eventBody)]);
    }

    public async Task PublishUnhandledExceptionAsync<TCommand>(
        MessageContainer<TCommand, CommandMetadata> commandContainer, UnhandledExceptionEvent eventBody)
        where TCommand : Message
    {
        await PublishEntriesAsync(commandContainer, [GetEntry(commandContainer, eventBody)]);
    }


    public async Task PublishUnhandledExceptionAsync<TSourceEvent>(
        MessageContainer<TSourceEvent, EventMetadata> eventContainer, UnhandledExceptionEvent eventBody)
        where TSourceEvent : Message
    {
        await PublishEntriesAsync(eventContainer, [GetEntry(eventContainer, eventBody)]);
    }


    /// <summary>
    /// Sends the entries to EventBridge (in batches of the PutEvents limit) and stores every
    /// entry that EventBridge accepted as a <see cref="PublishedEvent"/>. Results come back in
    /// the same order as the request entries, so they are matched by position. Each batch is
    /// stored in its own transaction, which completes the messaging unit of work once.
    /// </summary>
    private async Task PublishEntriesAsync<TMessage, TMetadata>(
        MessageContainer<TMessage, TMetadata> sourceContainer,
        List<RequestEntry> entries)
        where TMessage : Message
        where TMetadata : MessageMetadata
    {
        var sourceType = sourceContainer.Message.GetType().Name;

        foreach (var batch in entries.Chunk(MockEventBridgeClient.MaxEntriesPerRequest))
        {
            var response = await _eventBridgeClient.PutEventsAsync(new PutEventsRequest
            {
                Entries = batch.ToList()
            });

            var now = DateTimeOffset.UtcNow;
            var publishedEvents = new List<PublishedEvent>();

            foreach (var (requestEntry, resultEntry) in batch.Zip(response.Entries))
            {
                if (!resultEntry.IsSuccess)
                {
                    _logger.LogError(
                        "Failed to publish {DetailType} for {SourceType}: {ErrorCode} {ErrorMessage}",
                        requestEntry.DetailType, sourceType, resultEntry.ErrorCode, resultEntry.ErrorMessage);
                    continue;
                }

                using var payload = JsonDocument.Parse(requestEntry.Detail);

                publishedEvents.Add(new PublishedEvent
                {
                    Id = Guid.TryParse(resultEntry.EventId, out var eventId) ? eventId : Guid.NewGuid(),
                    Timestamp = now,
                    SystemTimestamp = now,
                    MessageType = requestEntry.DetailType,
                    Payload = payload.RootElement.Clone(),
                    CommandType = sourceType,
                    // TODO: no message id is carried in MessageMetadata yet, so CommandId stays Guid.Empty.
                });
            }

            if (publishedEvents.Count is 0)
            {
                continue;
            }

            await _transaction.ExecuteAsync(
                recorder => recorder.RecordAsync(publishedEvents),
                sourceContainer);
        }
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
                DetailType = typeof(TEvent).Name,
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
                DetailType = typeof(TEvent).Name,
                Detail = GetDetail(eventContainer, eventBody),
            }).ToList(); // List<PutEventsRequestEntry>
    }


    private RequestEntry GetEntry<TMessage>(
        MessageContainer<TMessage, CommandMetadata> commandContainer,
        AuthorizationFailedEvent eventBody)
        where TMessage : Message
    {
        return new RequestEntry
        {
            Source = "SourceName",
            DetailType = GetDetailType(commandContainer) + "AuthorizationFailedEvent",
            Detail = GetDetail(commandContainer, eventBody),
        };
    }

    private RequestEntry GetEntry<TMessage>(
        MessageContainer<TMessage, CommandMetadata> commandContainer,
        ValidationFailedEvent eventBody)
        where TMessage : Message
    {
        return new RequestEntry
        {
            Source = "SourceName",
            DetailType = GetDetailType(commandContainer) + "ValidationFailedEvent",
            Detail = GetDetail(commandContainer, eventBody),
        };
    }

    private RequestEntry GetEntry<TMessage>(
        MessageContainer<TMessage, EventMetadata> eventContainer,
        ValidationFailedEvent eventBody)
        where TMessage : Message
    {
        return new RequestEntry
        {
            Source = "SourceName",
            DetailType = GetDetailType(eventContainer) + "ValidationFailedEvent",
            Detail = GetDetail(eventContainer, eventBody),
        };
    }

    private RequestEntry GetEntry<TMessage>(
        MessageContainer<TMessage, CommandMetadata> commandContainer,
        UnhandledExceptionEvent eventBody)
        where TMessage : Message
    {
        return new RequestEntry
        {
            Source = "SourceName",
            DetailType = GetDetailType(commandContainer) + "FailedEvent",
            Detail = GetDetail(commandContainer, eventBody),
        };
    }

    private RequestEntry GetEntry<TMessage>(
        MessageContainer<TMessage, EventMetadata> eventContainer,
        UnhandledExceptionEvent eventBody)
        where TMessage : Message
    {
        return new RequestEntry
        {
            Source = "SourceName",
            DetailType = GetDetailType(eventContainer) + "FailedEvent",
            Detail = GetDetail(eventContainer, eventBody),
        };
    }

    private static string GetDetailType<TMessage>(MessageContainer<TMessage, CommandMetadata> commandContainer)
        where TMessage : Message
    {
        var name = commandContainer.Message.GetType().Name;
        return name[..name.LastIndexOf("Command", StringComparison.Ordinal)];
    }

    private static string GetDetailType<TMessage>(MessageContainer<TMessage, EventMetadata> eventContainer)
        where TMessage : Message
    {
        var name = eventContainer.Message.GetType().Name;
        return name[..name.LastIndexOf("Event", StringComparison.Ordinal)];
    }


    private string GetDetail<TMessage, TEvent>(
        MessageContainer<TMessage, CommandMetadata> commandContainer,
        TEvent eventBody)
        where TMessage : Message
        where TEvent : Message
    {
        return JsonSerializer.Serialize(new
        {
            body = eventBody,
            tags = GetTags(eventBody)
        });
    }

    private string GetDetail<TMessage, TEvent>(
        MessageContainer<TMessage, EventMetadata> eventContainer,
        TEvent eventBody)
        where TMessage : Message
        where TEvent : Message
    {
        return JsonSerializer.Serialize(new
        {
            body = eventBody,
            tags = GetTags(eventBody)
        });
    }

    private string GetDetail<TMessage>(
        MessageContainer<TMessage, CommandMetadata> commandContainer,
        AuthorizationFailedEvent eventBody)
        where TMessage : Message
    {
        return JsonSerializer.Serialize(new
        {
            body = eventBody,
            tags = GetTags(commandContainer).Concat(["authorization-failed"]).ToList()
        });
    }

    private string GetDetail<TMessage>(
        MessageContainer<TMessage, CommandMetadata> commandContainer,
        ValidationFailedEvent eventBody)
        where TMessage : Message
    {
        return JsonSerializer.Serialize(new
        {
            body = eventBody,
            tags = GetTags(commandContainer).Concat(["validation-failed"]).ToList()
        });
    }

    private string GetDetail<TMessage>(
        MessageContainer<TMessage, EventMetadata> eventContainer,
        ValidationFailedEvent eventBody)
        where TMessage : Message
    {
        return JsonSerializer.Serialize(new
        {
            body = eventBody,
            tags = GetTags(eventContainer).Concat(["validation-failed"]).ToList()
        });
    }

    private string GetDetail<TMessage>(
        MessageContainer<TMessage, CommandMetadata> commandContainer,
        UnhandledExceptionEvent eventBody)
        where TMessage : Message
    {
        return JsonSerializer.Serialize(new
        {
            body = eventBody,
            tags = GetTags(commandContainer).Concat(["failed"]).ToList()
        });
    }

    private string GetDetail<TMessage>(
        MessageContainer<TMessage, EventMetadata> eventContainer,
        UnhandledExceptionEvent eventBody)
        where TMessage : Message
    {
        return JsonSerializer.Serialize(new
        {
            body = eventBody,
            tags = GetTags(eventContainer).Concat(["failed"]).ToList()
        });
    }


    private static List<string> GetTags<TMessage>(MessageContainer<TMessage, CommandMetadata> commandContainer)
        where TMessage : Message
    {
        var tags =
            commandContainer.Message
                .GetType()
                .GetCustomAttributes(typeof(FailedMessageTagsAttribute))
                .OfType<FailedMessageTagsAttribute>()
                .SelectMany(attribute => attribute.Tags)
                .Select(tag => tag.ToLowerInvariant())
                .ToList();

        if (tags.Count is 0)
        {
            throw new InvalidOperationException(
                $"{commandContainer.GetType().Name} does not declare any tags with FailedMessageTagsAttribute.");
        }

        tags.Add("event");

        return tags.Distinct().ToList();
    }

    private static List<string> GetTags<TMessage>(MessageContainer<TMessage, EventMetadata> eventContainer)
        where TMessage : Message
    {
        var tags =
            eventContainer.Message
                .GetType()
                .GetCustomAttributes(typeof(FailedMessageTagsAttribute))
                .OfType<FailedMessageTagsAttribute>()
                .SelectMany(attribute => attribute.Tags)
                .Select(tag => tag.ToLowerInvariant())
                .ToList();

        if (tags.Count is 0)
        {
            throw new InvalidOperationException(
                $"{eventContainer.Message.GetType().Name} does not declare any tags with FailedMessageTagsAttribute.");
        }

        tags.Add("event");

        return tags.Distinct().ToList();
    }

    private static List<string> GetTags<TEvent>(TEvent eventBody)
    {
        var tags =
            eventBody!
                .GetType()
                .GetCustomAttributes(typeof(MessageTagsAttribute))
                .OfType<MessageTagsAttribute>()
                .SelectMany(attribute => attribute.Tags)
                .Select(tag => tag.ToLowerInvariant())
                .ToList();

        if (tags.Count is 0)
        {
            throw new InvalidOperationException(
                $"{typeof(TEvent).Name} does not declare any tags with MessageTagsAttribute.");
        }

        tags.Add("event");

        return tags.Distinct().ToList();
    }
}