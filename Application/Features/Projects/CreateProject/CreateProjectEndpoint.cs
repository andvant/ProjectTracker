using ProjectTracker.Application.Features.Projects.Common;

namespace ProjectTracker.Application.Features.Projects.CreateProject;

public record CreateProjectRequest(string ShortName, string Name);

public static class CreateProjectEndpoint
{
    public static void MapCreateProject(this IEndpointRouteBuilder app)
    {
        // POST /projects
        app.MapPost("/", async (
            CreateProjectRequest request,
            User user,
            ISender sender) =>
        {
            var command = new CreateProjectCommand(request.ShortName, request.Name, user);

            var project = await sender.Send(command);

            return TypedResults.CreatedAtRoute(
                project,
                EndpointNames.GetProject,
                new { id = project.Id });
        })
        .Produces<ProjectDto>(StatusCodes.Status201Created)
        .ProducesValidationProblem();
    }
}
