﻿using System.Reflection;
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

    public async Task PublishAuthorizationFailedAsync<TCommand>(
        MessageContainer<TCommand, CommandMetadata> commandContainer, AuthorizationFailedEvent eventBody)
        where TCommand : Message
    {
        var request = new PutEventsRequest()
        {
            Entries = [GetEntry(commandContainer, eventBody)]
        };

        request.Dump();
        await Task.Delay(250);
    }

    public async Task PublishValidationFailedAsync<TCommand>(
        MessageContainer<TCommand, CommandMetadata> commandContainer, ValidationFailedEvent eventBody)
        where TCommand : Message
    {
        var request = new PutEventsRequest()
        {
            Entries = [GetEntry(commandContainer, eventBody)]
        };

        request.Dump();
        await Task.Delay(250);
    }

    public async Task PublishValidationFailedAsync<TSourceEvent>(
        MessageContainer<TSourceEvent, EventMetadata> eventContainer, ValidationFailedEvent eventBody)
        where TSourceEvent : Message
    {
        var request = new PutEventsRequest()
        {
            Entries = [GetEntry(eventContainer, eventBody)]
        };

        request.Dump();
        await Task.Delay(250);
    }

    public async Task PublishUnhandledExceptionAsync<TCommand>(
        MessageContainer<TCommand, CommandMetadata> commandContainer, UnhandledExceptionEvent eventBody)
        where TCommand : Message
    {
        var request = new PutEventsRequest()
        {
            Entries = [GetEntry(commandContainer, eventBody)]
        };

        request.Dump();
        await Task.Delay(250);
    }


    public async Task PublishUnhandledExceptionAsync<TSourceEvent>(
        MessageContainer<TSourceEvent, EventMetadata> eventContainer, UnhandledExceptionEvent eventBody)
        where TSourceEvent : Message
    {
        var request = new PutEventsRequest()
        {
            Entries = [GetEntry(eventContainer, eventBody)]
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
            DetailType = GetEventName(typeof(TMessage)) + "AuthorizationFailedEvent",
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
            DetailType = GetEventName(typeof(TMessage)) + "ValidationFailedEvent",
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
            DetailType = GetEventName(typeof(TMessage)) + "ValidationFailedEvent",
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
            DetailType = GetEventName(typeof(TMessage)) + "FailedEvent",
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
            DetailType = GetEventName(typeof(TMessage)) + "FailedEvent",
            Detail = GetDetail(eventContainer, eventBody),
        };
    }

    private static string GetEventName(Type source)
    {
        var name = source.Name.AsSpan();
        var command = "Command".AsSpan();
        var @event = "Event".AsSpan();

        return name.EndsWith(command)
            ? name[..^command.Length].ToString()
            : name[..^@event.Length].ToString();
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
            tags = GetTags(typeof(TMessage)).Concat(["authorization-failed"]).ToList()
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
            tags = GetTags(typeof(TMessage)).Concat(["validation-failed"]).ToList()
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
            tags = GetTags(typeof(TMessage)).Concat(["validation-failed"]).ToList()
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
            tags = GetTags(typeof(TMessage)).Concat(["failed"]).ToList()
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

            tags = GetTags(typeof(TMessage)).Concat(["failed"]).ToList()
        });
    }


    private static List<string> GetTags(Type messageType)
    {
        var tags =
            messageType
                .GetCustomAttributes(typeof(FailedMessageTagsAttribute))
                .OfType<FailedMessageTagsAttribute>()
                .SelectMany(attribute => attribute.Tags)
                .Select(tag => tag.ToLowerInvariant())
                .ToList();

        if (tags.Count is 0)
        {
            throw new InvalidOperationException(
                $"{messageType.Name} does not declare any tags with MessageTagsAttribute.");
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