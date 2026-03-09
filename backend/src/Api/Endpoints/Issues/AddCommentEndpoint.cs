using ProjectTracker.Application.Features.Issues.AddComment;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Api.Endpoints.Issues;

public record AddCommentRequest(string Text, IssueStatus Status, Guid? AssigneeId);

internal static class AddCommentEndpoint
{
    public static void MapAddComment(this IEndpointRouteBuilder app)
    {
        // POST /projects/{projectId}/issues/{issueId}/comments
        app.MapPost("/{issueId:guid}/comments", async (
            Guid projectId,
            Guid issueId,
            AddCommentRequest request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new AddCommentCommand(projectId, issueId, request.Text, request.Status, request.AssigneeId);

            await sender.Send(command, ct);

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .ProducesValidationProblem();
    }
}
