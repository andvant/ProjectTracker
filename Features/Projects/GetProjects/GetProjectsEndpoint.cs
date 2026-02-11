using MediatR;
using ProjectTracker.Features.Projects.Common;

namespace ProjectTracker.Features.Projects.GetProjects;

public static class GetProjectsEndpoint
{
    public static void MapGetProjects(this IEndpointRouteBuilder app)
    {
        // GET /projects
        app.MapGet("/", async (ISender sender) =>
        {
            var query = new GetProjectsQuery();

            var projects = await sender.Send(query);

            return TypedResults.Ok(projects);
        })
        .Produces<List<ProjectDto>>(StatusCodes.Status200OK);
    }
}
