using ProjectTracker.Application.Features.Projects.DeleteProject;

namespace ProjectTracker.Api.Endpoints.Projects;

internal static class DeleteProjectEndpoint
{
    public static void MapDeleteProject(this IEndpointRouteBuilder app)
    {
        // DELETE /projects/{projectId}
        app.MapDelete("/{projectId}", async (Guid projectId, ISender sender, CancellationToken ct) =>
        {
            var command = new DeleteProjectCommand(projectId);

            await sender.Send(command, ct);

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
