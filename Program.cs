using System.Text.Json.Serialization;
using ProjectTracker.Features.Projects;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(opts =>
    opts.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

var user = new User("John Doe", "john.doe@mail.com");
var projects = GetProjects();

builder.Services.AddSingleton(projects);
builder.Services.AddSingleton(user);

var app = builder.Build();

app.MapProjects();

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
