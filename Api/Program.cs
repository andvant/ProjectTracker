using ProjectTracker.Api;
using ProjectTracker.Application;
using ProjectTracker.Application.Features.Projects;
using ProjectTracker.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();

var app = builder.Build();

app.MapProjects();

app.Run();
