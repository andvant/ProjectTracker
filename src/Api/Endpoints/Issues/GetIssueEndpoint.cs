using ProjectTracker.Application.Features.Issues.Common;
using ProjectTracker.Application.Features.Issues.GetIssue;

namespace ProjectTracker.Api.Endpoints.Issues;

internal static class GetIssueEndpoint
{
    public static void MapGetIssue(this IEndpointRouteBuilder app)
    {
        // GET /projects/{projectId}/issues/{issueId}
        app.MapGet("/{issueId:guid}", async (
            Guid projectId,
            Guid issueId,
            ISender sender) =>
        {
            var query = new GetIssueQuery(projectId, issueId);

            var issue = await sender.Send(query);

            return TypedResults.Ok(issue);
        })
        .WithName(EndpointNames.GetIssue)
        .Produces<IssueDto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
