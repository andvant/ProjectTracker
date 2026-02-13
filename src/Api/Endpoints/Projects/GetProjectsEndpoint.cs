using ProjectTracker.Application.Features.Projects.GetProjects;

namespace ProjectTracker.Api.Endpoints.Projects;

internal static class GetProjectsEndpoint
{
    public static void MapGetProjects(this IEndpointRouteBuilder app)
    {
        // GET /projects
        app.MapGet("/", async (ISender sender, CancellationToken ct) =>
        {
            var query = new GetProjectsQuery();

            var projects = await sender.Send(query, ct);

            return TypedResults.Ok(projects);
        })
        .Produces<List<ProjectsDto>>(StatusCodes.Status200OK);
    }
}
