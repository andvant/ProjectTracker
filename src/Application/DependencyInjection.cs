using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ProjectTracker.Application.Common.Behaviors;

namespace ProjectTracker.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();

        services.AddValidatorsFromAssembly(assembly);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
            cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddMappers(assembly);

        return services;
    }

    private static IServiceCollection AddMappers(this IServiceCollection services, Assembly assembly)
    {
        var mappers = assembly.GetTypes().Where(t =>
            t.IsClass &&
            !t.IsAbstract &&
            t.Name.EndsWith("Mapper"));

        foreach (var mapper in mappers)
        {
            services.AddSingleton(mapper);
        }

        return services;
    }
}
