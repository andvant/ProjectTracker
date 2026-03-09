using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectTracker.Application.Interfaces;
using ProjectTracker.Application.Interfaces.Caching;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Infrastructure.Caching;
using ProjectTracker.Infrastructure.Database;
using ProjectTracker.Infrastructure.Identity;
using ProjectTracker.Infrastructure.Notifications;
using ProjectTracker.Infrastructure.ObjectStorage;
using ZiggyCreatures.Caching.Fusion;
using ZiggyCreatures.Caching.Fusion.Serialization.SystemTextJson;

namespace ProjectTracker.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDevelopment)
    {
        services.Configure<S3Config>(configuration.GetSection(nameof(S3Config)));
        services.Configure<KeycloakAdminConfig>(configuration.GetSection(nameof(KeycloakAdminConfig)));

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ProjectTrackerDb"));
            options.EnableSensitiveDataLogging(isDevelopment);
            options.UseSnakeCaseNamingConvention();

            options.UseSeeding((context, _) => AddDefaultUsers(context));
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddFusionCache()
            .WithDistributedCache(new RedisCache(new RedisCacheOptions
            {
                Configuration = configuration.GetConnectionString("Redis")
            }))
            .WithSerializer(new FusionCacheSystemTextJsonSerializer())
            .WithDefaultEntryOptions(opts =>
            {
                opts.Duration = TimeSpan.FromMinutes(10);
            });

        services.AddSingleton<IAppCache, AppCache>();
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton<IObjectStorage, S3ObjectStorage>();
        services.AddSingleton<INotificationMessageFactory, NotificationMessageFactory>();

        return services;
    }

    public static void ApplyMigrations(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        if (context.Database.GetPendingMigrations().Any())
        {
            context.Database.Migrate();
        }
    }

    private static void AddDefaultUsers(DbContext context)
    {
        if (context.Set<User>().Any()) return;

        List<User> users =
        [
            new(Guid.Parse("9ee62ef4-159b-48d0-82c8-28e61bfacd14"),
                "admin", "alexei.alexeev@mail.ru", "Alexei Alexeev", DateTimeOffset.UtcNow),
            new(Guid.Parse("ab72d992-8648-4306-8bd9-4b4fb74a7d29"),
                "mgr", "maria.markova@mail.ru", "Maria Markova", DateTimeOffset.UtcNow),
            new(Guid.Parse("177afd42-4715-4631-946f-f532734c6389"),
                "user", "roman.romanov@mail.ru", "Roman Romanov", DateTimeOffset.UtcNow),
        ];

        context.Set<User>().AddRange(users);
        context.SaveChanges();
    }
}
