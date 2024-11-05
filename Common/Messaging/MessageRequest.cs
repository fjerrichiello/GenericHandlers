using System.Text.Json;

namespace Common.Messaging;

public record MessageRequest(string? DetailType, JsonElement? Detail);