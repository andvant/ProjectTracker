using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using ProjectTracker.Api;
using ProjectTracker.Api.Endpoints;
using ProjectTracker.Api.Identity;
using ProjectTracker.Api.OpenApi;
using ProjectTracker.Application;
using ProjectTracker.Infrastructure;
using ProjectTracker.ServiceDefaults;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment.IsDevelopment());
builder.Services.AddApiServices(builder.Configuration);

var app = builder.Build();

app.Services.ApplyMigrations();

app.UseExceptionHandler();
app.UseOpenApi();

app.UseAuthentication();
app.UseAuthorization();

app.UseMiddleware<UserProvisionMiddleware>();

app.MapHealthChecks("/healthz", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapDefaultEndpoints();

app.MapAllEndpoints();

app.Run();
