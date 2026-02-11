using ProjectTracker.Application.Features.Projects.DeleteProject;

namespace ProjectTracker.Api.Endpoints.Projects;

public static class DeleteProjectEndpoint
{
    public static void MapDeleteProject(this IEndpointRouteBuilder app)
    {
        // DELETE /projects/{projectId}
        app.MapDelete("/{projectId}", async (Guid projectId, ISender sender) =>
        {
            var command = new DeleteProjectCommand(projectId);

            await sender.Send(command);

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent);
    }
}
