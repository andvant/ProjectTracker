using System.Diagnostics;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.IdentityModel.Tokens;
using ProjectTracker.Api.ExceptionHandlers;
using ProjectTracker.Api.Identity;
using ProjectTracker.Api.OpenApi;
using ProjectTracker.Application.Interfaces;

namespace ProjectTracker.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
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

        services.AddScoped<UserProvisionMiddleware>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddSingleton<IContentTypeProvider, FileExtensionContentTypeProvider>();
        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 100 * 1024 * 1024; // 100 MB uploaded file size limit
        });

        services.AddHealthChecks()
            .AddNpgSql(configuration.GetConnectionString("ProjectTrackerDb")!);

        var keycloakConfig = configuration.GetSection(nameof(KeycloakConfig)).Get<KeycloakConfig>()!;

        services.AddOpenApiServices(keycloakConfig.Authority);

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
        });

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = keycloakConfig.RequireHttps;
                options.Audience = keycloakConfig.Audience;
                options.Authority = keycloakConfig.Authority;
                options.MetadataAddress = $"{keycloakConfig.Authority}/.well-known/openid-configuration";

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = keycloakConfig.Authority,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        return services;
    }
}
