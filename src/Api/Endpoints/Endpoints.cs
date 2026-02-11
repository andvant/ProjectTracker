using ProjectTracker.Api.Endpoints.Projects;
using ProjectTracker.Api.Endpoints.Issues;

namespace ProjectTracker.Api.Endpoints;

public static class EndpointNames
{
    public const string GetProject = nameof(GetProject);
    public const string GetIssue = nameof(GetIssue);
}

public static class Endpoints
{
    public static void MapProjects(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/projects");

        group.MapGetProject();
        group.MapGetProjects();
        group.MapCreateProject();
        group.MapUpdateProject();
        group.MapDeleteProject();
    }

    public static void MapIssues(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/projects/{projectId:guid}/issues");

        group.MapGetIssue();
        group.MapGetIssues();
        group.MapCreateIssue();
        group.MapUpdateIssue();
        group.MapDeleteIssue();
    }
}
