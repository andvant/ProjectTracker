using ProjectTracker.Application.Features.Issues.DeleteIssue;

namespace ProjectTracker.Api.Endpoints.Issues;

public static class DeleteIssueEndpoint
{
    public static void MapDeleteIssue(this IEndpointRouteBuilder app)
    {
        // DELETE /projects/{projectId}/issues/{issueId}
        app.MapDelete("/{issueId:guid}", async (Guid projectId, Guid issueId, ISender sender) =>
        {
            var command = new DeleteIssueCommand(projectId, issueId);

            await sender.Send(command);

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent);
    }
}
