using System.Text.Json;

namespace HttpSender;

public record MessageRequest(string? DetailType, Detail? Detail);

public record Detail(JsonElement? Body);