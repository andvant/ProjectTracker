using System.Text.Json.Serialization;
using ProjectTracker.Api.Middleware;

namespace ProjectTracker.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(opts =>
            opts.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        services.AddProblemDetails();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}
