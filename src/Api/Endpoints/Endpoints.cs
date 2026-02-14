using ProjectTracker.Api.Endpoints.Issues;
using ProjectTracker.Api.Endpoints.Projects;
using UserTracker.Api.Endpoints.Users;

namespace ProjectTracker.Api.Endpoints;

internal static class EndpointNames
{
    public const string GetProject = nameof(GetProject);
    public const string GetIssue = nameof(GetIssue);
}

internal static class Endpoints
{
    public static void MapAllEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapProjects();
        app.MapIssues();
        app.MapUsers();
    }

    private static void MapProjects(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/projects")
            .WithTags("Projects");

        group.MapGetProject();
        group.MapGetProjects();
        group.MapCreateProject();
        group.MapUpdateProject();
        group.MapDeleteProject();
    }

    private static void MapIssues(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/projects/{projectId:guid}/issues")
            .WithTags("Issues");

        group.MapGetIssue();
        group.MapGetIssues();
        group.MapCreateIssue();
        group.MapUpdateIssue();
        group.MapDeleteIssue();
    }

    private static void MapUsers(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users")
            .WithTags("Users");

        group.MapGetUser();
        group.MapGetUsers();
    }
}
