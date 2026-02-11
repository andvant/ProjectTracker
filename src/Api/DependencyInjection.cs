using System.Text.Json.Serialization;

namespace ProjectTracker.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(opts =>
            opts.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        return services;
    }
}
