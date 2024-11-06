using System.Net.Http.Json;
using System.Text.Json;
using HttpSender;
using HttpSender.Commands;

JsonSerializerOptions serializerOptions = new()
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
};
var httpClient = new HttpClient(new LoggingHandler(new HttpClientHandler()));
httpClient.BaseAddress = new Uri("http://localhost:5100");

const string genericCommandEventPath = "/generic-command-events";
const string structuredCommandEventPath = "/structured-command-events";

List<string> authors =
[
    "Dr. Seuss",
    // "Roald Dahl",
    // "Beatrix Potter",
    // "Maurice Sendak",
    // "Eric Carle",
    // "Shel Silverstein",
    // "Judy Blume"
];

foreach (var author in authors)
{
    var names = author.Split(" ", 2, StringSplitOptions.TrimEntries);
    var command = new AddAuthorCommand(names.First(), names.Last());
    var body = JsonSerializer.SerializeToElement(command, serializerOptions);
    var messageRequest = new MessageRequest(command.GetType().Name, new Detail(body));
    await httpClient.PostAsJsonAsync(genericCommandEventPath, messageRequest, serializerOptions);
}