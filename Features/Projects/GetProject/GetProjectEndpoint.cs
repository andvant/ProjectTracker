using ProjectTracker.Features.Projects.Common;

namespace ProjectTracker.Features.Projects.GetProject;

public static class GetProjectEndpoint
{
    public static void MapGetProject(this IEndpointRouteBuilder app)
    {
        // GET /projects/{id}
        app.MapGet("/{id}", async (
            Guid id,
            [FromServices] List<Project> projects) =>
        {
            var project = projects.FirstOrDefault(p => p.Id == id);

            if (project is null)
            {
                return Results.NotFound();
            }

            return TypedResults.Ok(new ProjectDto(project.Id, project.ShortName, project.Name, project.Description));
        })
        .WithName(EndpointNames.GetProject)
        .Produces<ProjectDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
