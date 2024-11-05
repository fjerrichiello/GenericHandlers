using System.Text.Json;

namespace Common.Messaging;

public class MessageContainerMapper<TMessage, TMessageMetadata>()
    where TMessage : Message
    where TMessageMetadata : MessageMetadata
{
    public MessageContainer<TMessage, TMessageMetadata>
        ToMessageContainer(MessageRequest request)
    {
        var detail = request.Detail
                     ?? throw new ArgumentNullException(
                         nameof(request),
                         $"{nameof(request)}.{nameof(request.Detail)} must not be null.");

        var messageContent = (IEnumerable<JsonElement>)GetMessageContent(detail);

        using var mergedMessageContent = (JsonDocument)Merge(messageContent);

        var message = mergedMessageContent.Deserialize<TMessage>(new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        var metadata = detail.Deserialize<TMessageMetadata>();

        return new MessageContainer<TMessage, TMessageMetadata>(
            message!,
            // As long as request.Detail is any kind of JSON object then this 
            // value will be not null. If the request.Detail is any value that 
            // can't be converted, then a JsonException would be thrown.
            metadata!);
    }


    private static IEnumerable<JsonElement>
        GetMessageContent(JsonElement detail)
    {
        if (detail.TryGetProperty("body", out var body))
        {
            yield return body;
        }

        if (!detail.TryGetProperty("parameters", out var parameters))
        {
            yield break;
        }

        if (parameters.TryGetProperty("path", out var path))
        {
            yield return path;
        }

        if (parameters.TryGetProperty("querystring", out var querystring))
        {
            yield return querystring;
        }
    }

    private static JsonDocument Merge(IEnumerable<JsonElement> messageContent)
    {
        using var memoryStream = new MemoryStream();
        using var jsonWriter = new Utf8JsonWriter(memoryStream);

        var properties = messageContent
            .SelectMany(content => content.EnumerateObject())
            .GroupBy(property => property.Name)
            .Select(grouping => grouping.LastOrDefault());

        jsonWriter.WriteStartObject();

        foreach (var property in properties)
        {
            property.WriteTo(jsonWriter);
        }

        jsonWriter.WriteEndObject();
        jsonWriter.Flush();

        memoryStream.Position = 0;

        return JsonDocument.Parse(memoryStream);
    }
}