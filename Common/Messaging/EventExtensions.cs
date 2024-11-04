using System.Reflection;

namespace Common.Messaging;

public static class EventExtensions
{
    public static List<string> GetGenericFailedTags(Type eventType)
    {
        var tags = GetTags(eventType);
        tags.Remove("failed");
        tags.Remove("Failed");
        return tags;
    }

    public static List<string> GetTags(Type eventType)
    {
        var tags =
            eventType
                .GetCustomAttributes(typeof(MessageTagsAttribute))
                .OfType<MessageTagsAttribute>()
                .SelectMany(attribute => attribute.Tags)
                .Select(tag => tag.ToLowerInvariant())
                .ToList();

        if (tags.Count is 0)
        {
            throw new InvalidOperationException(
                $"{eventType.Name} does not declare any tags with MessageTagsAttribute.");
        }

        tags.Add("event");

        return tags.Distinct().ToList();
    }
}