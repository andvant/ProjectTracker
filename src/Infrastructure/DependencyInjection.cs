using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectTracker.Application.Common;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.Enums;
using ProjectTracker.Infrastructure.Database;

namespace ProjectTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDevelopment)
    {
        services.AddSingleton(TimeProvider.System);

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite(configuration.GetConnectionString("ProjectTrackerDb"));
            options.EnableSensitiveDataLogging(isDevelopment);
            options.UseSnakeCaseNamingConvention();

            options.UseSeeding((context, _) =>
            {
                var user = GetUsers()[0];

                var existing = context.Set<User>().Find(user.Id);

                if (existing == null)
                {
                    context.Set<User>().Add(user);
                    context.SaveChanges();
                }
            });
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    private static List<Project> GetProjects(User user)
    {
        var projects = new List<Project>()
        {
            new("P1", "Project One", user, "Description of Project One", DateTimeOffset.UtcNow),
            new("P2", "Project Two", user, "Description of Project Two", DateTimeOffset.UtcNow)
        };

        projects[0].CreateIssue(1, "Issue One", "Description of Issue One",
            user, null, IssueType.Task, IssuePriority.Normal, null, null, DateTimeOffset.UtcNow, null);
        projects[0].CreateIssue(2, "Issue Two", "Description of Issue Two",
            user, user, IssueType.Bug, IssuePriority.Critical, null, null, DateTimeOffset.UtcNow, null);

        return projects;
    }

    private static List<User> GetUsers() => new List<User>()
    {
        new("Test User 1", "test.1@example.com", DateTimeOffset.UtcNow.AddDays(-3)),
        new("Test User 2", "test.2@example.com", DateTimeOffset.UtcNow.AddDays(-2)),
        new("Test User 3", "test.3@example.com", DateTimeOffset.UtcNow.AddDays(-1)),
    };
}
