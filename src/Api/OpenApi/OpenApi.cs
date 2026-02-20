using Microsoft.OpenApi;
using Scalar.AspNetCore;

namespace ProjectTracker.Api.OpenApi;

internal static class OpenApi
{
    public static IServiceCollection AddOpenApiServices(this IServiceCollection services, string keycloakAuthority)
    {
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Keycloak", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.OAuth2,
                Flows = new OpenApiOAuthFlows
                {
                    AuthorizationCode = new OpenApiOAuthFlow
                    {
                        AuthorizationUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/auth"),
                        TokenUrl = new Uri($"{keycloakAuthority}/protocol/openid-connect/token"),
                        Scopes = new Dictionary<string, string>
                        {
                            ["openid"] = "OpenID Connect scope",
                            ["profile"] = "User profile scope",
                        }
                    }
                }
            });

            options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecuritySchemeReference("Keycloak", doc),
                    []
                }
            });
        });

        return services;
    }

    public static IApplicationBuilder UseOpenApi(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment()) return app;

        app.MapOpenApi();
        app.MapScalarApiReference();
        app.UseSwagger();
        app.UseSwaggerUI(o =>
        {
            o.OAuthClientId(app.Configuration["KeycloakConfig:ClientId"]);
            o.OAuthUsePkce();
        });

        return app;
    }
}
