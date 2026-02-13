using ProjectTracker.Application.Features.Issues.GetIssues;

namespace ProjectTracker.Api.Endpoints.Issues;

internal static class GetIssuesEndpoint
{
    public static void MapGetIssues(this IEndpointRouteBuilder app)
    {
        // GET /projects/{projectId}/issues
        app.MapGet("/", async (Guid projectId, ISender sender) =>
        {
            var query = new GetIssuesQuery(projectId);

            var issues = await sender.Send(query);

            return TypedResults.Ok(issues);
        })
        .Produces<List<IssuesDto>>(StatusCodes.Status200OK);
    }
}
