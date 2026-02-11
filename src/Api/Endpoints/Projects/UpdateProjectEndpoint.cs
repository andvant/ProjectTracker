using ProjectTracker.Application.Features.Projects.UpdateProject;

namespace ProjectTracker.Api.Endpoints.Projects;

public record UpdateProjectRequest(string Name, string? Description);

public static class UpdateProjectEndpoint
{
    public static void MapUpdateProject(this IEndpointRouteBuilder app)
    {
        // PUT /projects/{id}
        app.MapPut("/{id}", async (
            Guid id,
            UpdateProjectRequest request,
            ISender sender,
            ILogger<Program> logger) =>
        {
            var command = new UpdateProjectCommand(id, request.Name, request.Description);

            await sender.Send(command);

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .ProducesValidationProblem();
    }
}
