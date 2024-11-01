using CommonWithEventFactories;
using CommonWithEventFactories.Messaging;
using GenericHandlersWithEventFactories.CommandHandlers.AddCommandHandler;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddEventHandlersWithEventFactoryAndNecessaryWork(typeof(AddCommandOperation));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/command-events",
        async ([FromBody] MessageRequest request, IServiceProvider _provider) =>
        {
            var orchestrator = _provider.GetRequiredKeyedService<IMessageOrchestrator>(request.DetailType);

            await orchestrator.ProcessAsync(request);
        })
    .WithName("TestOrchestrator")
    .WithOpenApi();

app.Run();

//{
// "source": "string",
// "detailType": "AddCommand",
// "detail": {
//     "body":{
//         "Value1":1
//     }
// }
// }


//{
// "source": "string",
// "detailType": "RemoveCommand",
// "detail": {
//     "body":{
//         "Value1":2
//     }
// }
// }

//{
// "source": "string",
// "detailType": "DividedEvent",
// "detail": {
//     "body":{
//         "Value1":3
//     }
// }
// }

// {
//  "source": "string",
//  "detailType": "MultipliedEvent",
//  "detail": {
//      "body":{
//          "Value1":4
//      }
//  }