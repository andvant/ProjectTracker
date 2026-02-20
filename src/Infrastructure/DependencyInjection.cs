using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectTracker.Application.Interfaces;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Infrastructure.Database;
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

        var user = new User(Guid.CreateVersion7(), "Default User 1", "test1@example.com", DateTimeOffset.UtcNow.AddDays(-3));
        context.Set<User>().Add(user);
        context.SaveChanges();
    }
}
