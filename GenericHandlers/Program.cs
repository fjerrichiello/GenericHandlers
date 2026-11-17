using Common;
using GenericHandlers;
using GenericHandlers.CommandHandlers.Authors.AddAuthor;
using GenericHandlers.MessagingPersistence;
using GenericHandlers.Persistence;
using GenericHandlers.Persistence.Repositories;
using GenericHandlers.Persistence.UnitOfWork;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.AddEventHandlersAndNecessaryWork(typeof(AddAuthorOperation));


// Add Services

services.AddScoped<IBookRepository, BookRepository>();

services.AddScoped<IAuthorRepository, AuthorRepository>();

services.AddScoped<IBookRequestRepository, BookRequestRepository>();

services.AddScoped<IUnitOfWork, UnitOfWork<ApplicationDbContext>>();


services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("HandlersDatabase")));

services.AddDbContext<ApplicationMessagingDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("MessagingDatabase")));

services.AddMessagingPersistence<ApplicationMessagingDbContext, MessagingUnitOfWork>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.AddGenericEndpoint();


app.Run();

// {
//  "source": "string",
//  "detailType": "AddCommand",
//  "detail": {
//      "body":{
//          "Value1":1
//      }
//  }
//  }


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
//  }