using System.Text.Json;

namespace Common.Structured.Messaging;

public record MessageRequest(string? DetailType, JsonElement? Detail);