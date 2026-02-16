using ProjectTracker.Api;
using ProjectTracker.Api.Endpoints;
using ProjectTracker.Api.Middleware;
using ProjectTracker.Application;
using ProjectTracker.Infrastructure;
using ProjectTracker.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();
builder.Services.AddApiServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
}

app.UseExceptionHandler();
app.UseOpenApi();

app.MapAllEndpoints();

app.Run();
