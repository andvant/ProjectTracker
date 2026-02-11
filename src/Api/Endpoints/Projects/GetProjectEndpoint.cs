using ProjectTracker.Application.Features.Projects.Common;
using ProjectTracker.Application.Features.Projects.GetProject;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Api.Endpoints.Projects;

public static class GetProjectEndpoint
{
    public static void MapGetProject(this IEndpointRouteBuilder app)
    {
        // GET /projects/{projectId}
        app.MapGet("/{projectId}", async (
            Guid projectId,
            ISender sender,
            List<Project> projects) =>
        {
            var query = new GetProjectQuery(projectId);

            var project = await sender.Send(query);

            return project is null
                ? Results.NotFound()
                : TypedResults.Ok(project);
        })
        .WithName(EndpointNames.GetProject)
        .Produces<ProjectDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
