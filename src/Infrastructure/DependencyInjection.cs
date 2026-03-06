using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectTracker.Application.Interfaces;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Infrastructure.Database;
using ProjectTracker.Infrastructure.Identity;
using ProjectTracker.Infrastructure.ObjectStorage;

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

            options.UseSeeding((context, _) => AddDefaultUser(context));
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.Configure<S3Config>(configuration.GetSection(nameof(S3Config)));
        services.AddSingleton<IObjectStorage, S3ObjectStorage>();

        services.Configure<KeycloakAdminConfig>(configuration.GetSection(nameof(KeycloakAdminConfig)));

        return services;
    }

    public static Guid ApplyMigrations(this IServiceProvider serviceProvider)
    {
        Guid defaultUserId;

        using (var scope = serviceProvider.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            defaultUserId = context.Users.OrderBy(u => u.Id).First().Id;
        }

        return defaultUserId;
    }

    private static void AddDefaultUser(DbContext context)
    {
        if (context.Set<User>().Any()) return;

        var user = new User(Guid.Parse("9ee62ef4-159b-48d0-82c8-28e61bfacd14"),
            "admin", "admin@mail.com", "John Doe", DateTimeOffset.UtcNow);
        context.Set<User>().Add(user);
        context.SaveChanges();
    }
}
