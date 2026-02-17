using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectTracker.Application.Common;
using ProjectTracker.Domain.Entities;
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
            options.UseNpgsql(configuration.GetConnectionString("ProjectTrackerDb"));
            options.EnableSensitiveDataLogging(isDevelopment);
            options.UseSnakeCaseNamingConvention();

            options.UseSeeding((context, _) =>
            {
                var existing = context.Set<User>().OrderBy(u => u.Id).FirstOrDefault();

                if (existing is null)
                {
                    var user = new User("Default User 1", "test1@example.com", DateTimeOffset.UtcNow.AddDays(-3));
                    context.Set<User>().Add(user);
                    context.SaveChanges();
                }
            });
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        return services;
    }

    public static Guid ApplyMigrations(this IServiceProvider serviceProvider)
    {
        Guid defaultUserId;

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();

            defaultUserId = context.Users.OrderBy(u => u.Id).First().Id;
        }

        return defaultUserId;
    }
}
