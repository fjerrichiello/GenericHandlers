using System.Text.Json;

namespace HttpSender;

public class MessageRequest(string? DetailType, JsonElement? Detail);