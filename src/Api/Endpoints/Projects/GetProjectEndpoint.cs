using ProjectTracker.Application.Features.Projects.Common;
using ProjectTracker.Application.Features.Projects.GetProject;

namespace ProjectTracker.Api.Endpoints.Projects;

internal static class GetProjectEndpoint
{
    public static void MapGetProject(this IEndpointRouteBuilder app)
    {
        // GET /projects/{projectId}
        app.MapGet("/{projectId}", async (
            Guid projectId,
            ISender sender,
            CancellationToken ct) =>
        {
            var query = new GetProjectQuery(projectId);

            var project = await sender.Send(query, ct);

            return TypedResults.Ok(project);
        })
        .WithName(EndpointNames.GetProject)
        .Produces<ProjectDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
