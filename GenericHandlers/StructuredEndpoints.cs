using Common.Structured.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace GenericHandlers;

public static class StructuredEndpoints
{
    public static void AddStructuredEndpoint(this WebApplication app)
    {
        app.MapPost("/structured-command-events",
                async ([FromBody] MessageRequest request, IServiceProvider _provider) =>
                {
                    var orchestrator =
                        _provider.GetRequiredKeyedService<IMessageOrchestrator>(
                            request.DetailType);

                    await orchestrator.ProcessAsync(request);
                })
            .WithName("TestStructuredOrchestrator")
            .WithOpenApi();
    }
}