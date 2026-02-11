using ProjectTracker.Api;
using ProjectTracker.Api.Endpoints;
using ProjectTracker.Application;
using ProjectTracker.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddApiServices();

var app = builder.Build();

app.MapProjects();

app.Run();
