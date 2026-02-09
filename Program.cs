using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(opts =>
    opts.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var app = builder.Build();

var user = new User("John Doe", "john.doe@mail.com");
var projects = GetProjects();

app.MapGet("/projects", async () => TypedResults.Ok(projects));

app.MapPost("/projects", async (CreateProjectRequest request) =>
{
    var project = new Project(request.ShortName, request.Name, user);
    projects.Add(project);

    return TypedResults.Created();
});

app.MapDelete("/projects/{id}", async (Guid id) =>
{
    var project = projects.FirstOrDefault(p => p.Id == id);

    if (project is not null)
    {
        projects.Remove(project);
    }

    return TypedResults.NoContent();
});

app.MapPut("/projects/{id}", async (Guid id, UpdateProjectRequest request) =>
{
    var project = projects.FirstOrDefault(p => p.Id == id);

    if (project is not null)
    {
        project.Name = request.Name;
        project.Description = request?.Description;
    }

    return TypedResults.NoContent();
});

app.Run();

List<Project> GetProjects()
{
    var projects = new List<Project>()
    {
        new Project("P1", "Project One", user)
        {
            Description = "Description of Project One"
        },
        new Project("P2", "Project Two", user)
        {
            Description = "Description for Project Two"
        }
    };

    var issues = new List<Issue>()
    {
        new(1, "Issue One", projects[0], user, user),
        new(1, "Issue Two", projects[0], user, priority: IssuePriority.Critical)
    };

    projects[0].AddIssue(issues[0]);
    projects[0].AddIssue(issues[1]);

    return projects;
}
