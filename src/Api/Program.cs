using System.Security.Claims;
using ProjectTracker.Api;
using ProjectTracker.Api.Endpoints;
using ProjectTracker.Api.Middleware;
using ProjectTracker.Application;
using ProjectTracker.Infrastructure;
using ProjectTracker.Infrastructure.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration, builder.Environment.IsDevelopment());
builder.Services.AddApiServices();

var app = builder.Build();

app.UseExceptionHandler();
app.UseOpenApi();

Guid defaultUserId;

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    //context.Database.EnsureDeleted();
    context.Database.EnsureCreated();
    defaultUserId = context.Users.First().Id;
}

app.Use(async (context, next) =>
{
    Claim[] claims = [new Claim(ClaimTypes.NameIdentifier, defaultUserId.ToString())];

    var identity = new ClaimsIdentity(claims);
    context.User.AddIdentity(identity);

    await next();
});

app.MapAllEndpoints();

app.Run();
