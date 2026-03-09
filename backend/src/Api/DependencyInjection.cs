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
using ProjectTracker.Infrastructure.Identity;

namespace ProjectTracker.Api;

public static class DependencyInjection
{
    private const long UploadFileSizeLimit = 100 * 1024 * 1024; // 100 MB

    public static IServiceCollection AddApiServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IWebHostBuilder webHost)
    {
        services.ConfigureHttpJsonOptions(opts =>
            opts.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

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

        webHost.ConfigureKestrel(options =>
        {
            options.Limits.MaxRequestBodySize = UploadFileSizeLimit;
        });
        services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = UploadFileSizeLimit;
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
                options.MetadataAddress = keycloakConfig.MetadataAddress;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = keycloakConfig.Authority,
                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

        services.AddHttpClient<IIdentityService, KeycloakService>(c =>
        {
            c.BaseAddress = new Uri(configuration["KeycloakAdminConfig:BaseUrl"]!);
        });

        services.AddCors(options =>
            options.AddPolicy("web", policy =>
            {
                policy.WithOrigins(configuration["CorsConfig:Origins"]!)
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            })
        );

        return services;
    }
}
