using System.Text.Json;

namespace Common.Messaging;

public record MessageRequest(string? Source, string? DetailType, JsonElement? Detail);