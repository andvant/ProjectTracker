using ProjectTracker.Application.Common;
using ProjectTracker.Application.Features.Projects.CreateProject;
using ProjectTracker.Application.Features.Projects.GetProject;

namespace ProjectTracker.Api.Endpoints.Projects;

public record CreateProjectRequest(string Key, string Name, string? Description);

internal static class CreateProjectEndpoint
{
    public static void MapCreateProject(this IEndpointRouteBuilder app)
    {
        // POST /projects
        app.MapPost("/", async (
            CreateProjectRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateProjectCommand(request.Key, request.Name, request.Description);

            var project = await sender.Send(command, ct);

            return TypedResults.CreatedAtRoute(
                project,
                EndpointNames.GetProject,
                new { projectId = project.Id });
        })
        .RequireAuthorization(p => p.RequireRole(Roles.ProjectManager))
        .Produces<ProjectDto>(StatusCodes.Status201Created)
        .ProducesValidationProblem();
    }
}
