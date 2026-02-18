using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.StaticFiles;
using ProjectTracker.Api.Identity;
using ProjectTracker.Api.Middleware;
using ProjectTracker.Application.Interfaces;

namespace ProjectTracker.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(opts =>
            opts.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

        services.AddProblemDetails(opts =>
        {
            opts.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
                context.ProblemDetails.Extensions["traceId"] = Activity.Current?.TraceId.ToString();
                context.ProblemDetails.Extensions["requestId"] = context.HttpContext.TraceIdentifier;
            };
        });
        services.AddExceptionHandler<ValidationExceptionHandler>();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddOpenApiServices();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();

        return services;
    }
}
