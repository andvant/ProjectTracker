using Microsoft.Extensions.DependencyInjection;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        var users = GetUsers();
        var projects = GetProjects(users[0]);

        services.AddSingleton(users);
        services.AddSingleton(projects);
        services.AddSingleton(users[0]);

        return services;
    }

    private static List<Project> GetProjects(User user)
    {
        var projects = new List<Project>()
        {
            new("P1", "Project One", user, "Description of Project One"),
            new("P2", "Project Two", user, "Description of Project Two")
        };

        projects[0].CreateIssue(1, "Issue One", "Description of Issue One",
            user, null, IssueType.Task, IssuePriority.Normal, null, null, null);
        projects[0].CreateIssue(2, "Issue Two", "Description of Issue Two",
            user, user, IssueType.Bug, IssuePriority.Critical, null, null, null);

        return projects;
    }

    private static List<User> GetUsers() => new List<User>()
    {
        new("Test User 1", "test.1@example.com", DateTime.UtcNow.AddDays(-3)),
        new("Test User 2", "test.2@example.com", DateTime.UtcNow.AddDays(-2)),
        new("Test User 3", "test.3@example.com", DateTime.UtcNow.AddDays(-1)),
    };
}
