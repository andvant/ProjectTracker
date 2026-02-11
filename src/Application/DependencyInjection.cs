using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using ProjectTracker.Application.Common.Behaviors;
using ProjectTracker.Application.Features.Issues.Common;
using ProjectTracker.Application.Features.Issues.GetIssues;
using ProjectTracker.Application.Features.Projects.Common;
using ProjectTracker.Application.Features.Projects.GetProjects;

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

        services.AddSingleton<ProjectDtoMapper>();
        services.AddSingleton<ProjectsDtoMapper>();
        services.AddSingleton<IssueDtoMapper>();
        services.AddSingleton<IssuesDtoMapper>();

        return services;
    }
}
