using ProjectTracker.Application.Features.Projects.Common;

namespace ProjectTracker.Application.Features.Projects.GetProject;

public static class GetProjectEndpoint
{
    public static void MapGetProject(this IEndpointRouteBuilder app)
    {
        // GET /projects/{id}
        app.MapGet("/{id}", async (
            Guid id,
            ISender sender,
            List<Project> projects) =>
        {
            var query = new GetProjectQuery(id);

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
