using ProjectTracker.Features.Projects.Common;

namespace ProjectTracker.Features.Projects.GetProjects;

public static class GetProjectsEndpoint
{
    public static void MapGetProjects(this IEndpointRouteBuilder app)
    {
        // GET /projects
        app.MapGet("/", async (
            [FromServices] List<Project> projects,
            ProjectMapper mapper,
            ILogger<Program> logger) =>
        {
            return TypedResults.Ok(projects.Select(p => mapper.ToDto(p)));
        })
        .Produces<List<ProjectDto>>(StatusCodes.Status200OK);
    }
}
