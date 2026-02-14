using Microsoft.Extensions.DependencyInjection;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        var user = new User("John Doe", "john.doe@mail.com", DateTime.UtcNow);
        var projects = GetProjects(user);
        List<User> users = [user];

        services.AddSingleton(user);
        services.AddSingleton(projects);
        services.AddSingleton(users);

        return services;
    }

    private static List<Project> GetProjects(User user)
    {
        var projects = new List<Project>()
        {
            new("P1", "Project One", user, "Description of Project One"),
            new("P2", "Project Two", user, "Description of Project Two")
        };

        projects[0].CreateIssue(1, "Issue One", user, null, IssueType.Task, IssuePriority.Normal, null, null, null);
        projects[0].CreateIssue(2, "Issue Two", user, user, IssueType.Bug, IssuePriority.Critical, null, null, null);

        return projects;
    }
}
