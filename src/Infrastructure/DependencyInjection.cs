using Microsoft.Extensions.DependencyInjection;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        var user = new User("John Doe", "john.doe@mail.com");
        var projects = GetProjects(user);

        services.AddSingleton(user);
        services.AddSingleton(projects);

        return services;
    }

    private static List<Project> GetProjects(User user)
    {
        var projects = new List<Project>()
        {
            new("P1", "Project One", user)
            {
                Description = "Description of Project One"
            },
            new("P2", "Project Two", user)
            {
                Description = "Description of Project Two"
            }
        };

        projects[0].CreateIssue(1, "Issue One", user, user);
        projects[0].CreateIssue(2, "Issue Two", user, priority: IssuePriority.Critical);

        return projects;
    }
}
