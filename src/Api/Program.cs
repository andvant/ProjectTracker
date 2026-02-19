using System.Security.Claims;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using ProjectTracker.Api;
using ProjectTracker.Api.Endpoints;
using ProjectTracker.Api.Middleware;
using ProjectTracker.Application;
using ProjectTracker.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment.IsDevelopment());
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();
app.UseOpenApi();

var defaultUserId = app.Services.ApplyMigrations();

app.Use(async (context, next) =>
{
    Claim[] claims = [new Claim(ClaimTypes.NameIdentifier, defaultUserId.ToString())];

    var identity = new ClaimsIdentity(claims);
    context.User.AddIdentity(identity);

    await next();
});

app.MapHealthChecks("/healthz", new HealthCheckOptions()
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapAllEndpoints();

app.Run();
