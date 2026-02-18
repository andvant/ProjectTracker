using ProjectTracker.Application.Features.Projects.UpdateProject;

namespace ProjectTracker.Api.Endpoints.Projects;

public record UpdateProjectRequest(string Name, string? Description);

internal static class UpdateProjectEndpoint
{
    public static void MapUpdateProject(this IEndpointRouteBuilder app)
    {
        // PUT /projects/{projectId}
        app.MapPut("/{projectId:guid}", async (
            Guid projectId,
            UpdateProjectRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateProjectCommand(projectId, request.Name, request.Description);

            await sender.Send(command, ct);

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .ProducesValidationProblem();
    }
}
