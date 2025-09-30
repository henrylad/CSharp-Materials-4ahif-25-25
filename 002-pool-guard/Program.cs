using Microsoft.AspNetCore.Mvc;
using PoolGuard.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer(); // Add API Explorer
builder.Services.AddOpenApi();
builder.Services.RegisterServices();
builder.Services.AddCors();
builder.Services.ConfigureCors();
builder.Services.ConfigureServices(builder.Environment.IsDevelopment());

var app = builder.Build();

if(builder.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors(Setup.CorsPolicyName);

app.MapGet("/hello/{name}", ([FromRoute] string? name,[FromServices] IClock clock) =>
   {
       if (string.IsNullOrWhiteSpace(name))
       {
           return Results.BadRequest();
       }

       var now = clock.GetCurrentInstant();

       return Results.Ok(new Greeting(name, now));
   })
   .Produces<Greeting>(StatusCodes.Status200OK)
   .Produces(StatusCodes.Status400BadRequest)
   .WithName("Greetings")
   .WithOpenApi();

await app.RunAsync();

internal sealed class Greeting(string name, Instant timestamp)
{
    public string Message => $"Hello, {name}!";
    public Instant Timestamp => timestamp;
}
