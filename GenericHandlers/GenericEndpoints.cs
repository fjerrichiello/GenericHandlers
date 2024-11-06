using Common.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace GenericHandlers;

public static class GenericEndpoints
{
    public static void AddGenericEndpoint(this WebApplication app)
    {
        app.MapPost("/generic-command-events",
                async ([FromBody] MessageRequest request, IServiceProvider _provider) =>
                {
                    var orchestrator =
                        _provider.GetRequiredKeyedService<IMessageOrchestrator>(
                            request.DetailType);

                    await orchestrator.ProcessAsync(request);
                })
            .WithName("TestGenericOrchestrator")
            .WithOpenApi();
    }
}