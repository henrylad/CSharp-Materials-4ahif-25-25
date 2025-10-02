using Microsoft.AspNetCore.Mvc;
using PoolGuard.Core;
using PoolGuard.Core.Tickets;

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



app.MapTicketEndpoints();

await app.RunAsync();

internal sealed class Greeting(string name, Instant timestamp)
{
    public string Message => $"Hello, {name}!";
    public Instant Timestamp => timestamp;
}
