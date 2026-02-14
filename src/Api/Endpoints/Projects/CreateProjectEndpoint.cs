using ProjectTracker.Application.Features.Projects.CreateProject;
using ProjectTracker.Application.Features.Projects.GetProject;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Api.Endpoints.Projects;

public record CreateProjectRequest(string Key, string Name, string? Description);

internal static class CreateProjectEndpoint
{
    public static void MapCreateProject(this IEndpointRouteBuilder app)
    {
        // POST /projects
        app.MapPost("/", async (
            CreateProjectRequest request,
            User user,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateProjectCommand(request.Key, request.Name, user, request.Description);

            var project = await sender.Send(command, ct);

            return TypedResults.CreatedAtRoute(
                project,
                EndpointNames.GetProject,
                new { projectId = project.Id });
        })
        .Produces<ProjectDto>(StatusCodes.Status201Created)
        .ProducesValidationProblem();
    }
}
