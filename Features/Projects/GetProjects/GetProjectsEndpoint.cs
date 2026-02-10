using ProjectTracker.Features.Projects.Common;

namespace ProjectTracker.Features.Projects.GetProjects;

public static class GetProjectsEndpoint
{
    public static void MapGetProjects(this IEndpointRouteBuilder app)
    {
        // GET /projects
        app.MapGet("/", async (
            [FromServices] List<Project> projects,
            ILogger<Program> logger) =>
        {
            return TypedResults.Ok(projects.Select(p => new ProjectDto(p.Id, p.ShortName, p.Name, p.Description)));
        })
        .Produces<List<ProjectDto>>(StatusCodes.Status200OK);
    }
}
