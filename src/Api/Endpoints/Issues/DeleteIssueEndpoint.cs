using ProjectTracker.Application.Features.Issues.DeleteIssue;

namespace ProjectTracker.Api.Endpoints.Issues;

internal static class DeleteIssueEndpoint
{
    public static void MapDeleteIssue(this IEndpointRouteBuilder app)
    {
        // DELETE /projects/{projectId}/issues/{issueId}
        app.MapDelete("/{issueId:guid}", async (Guid projectId, Guid issueId, ISender sender, CancellationToken ct) =>
        {
            var command = new DeleteIssueCommand(projectId, issueId);

            await sender.Send(command, ct);

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
