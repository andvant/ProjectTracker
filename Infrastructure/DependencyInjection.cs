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

        var issues = new List<Issue>()
        {
            new(1, "Issue One", projects[0], user, user),
            new(2, "Issue Two", projects[0], user, priority: IssuePriority.Critical)
        };

        projects[0].AddIssue(issues[0]);
        projects[0].AddIssue(issues[1]);

        return projects;
    }
}
