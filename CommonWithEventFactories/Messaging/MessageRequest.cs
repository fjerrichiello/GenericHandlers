using System.Text.Json;

namespace CommonWithEventFactories.Messaging;

public record MessageRequest(string? Source, string? DetailType, JsonElement? Detail);