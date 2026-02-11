using ProjectTracker.Api.Endpoints.Projects;

namespace ProjectTracker.Api.Endpoints;

public static class EndpointNames
{
    public const string GetProject = nameof(GetProject);
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
}
