using ProjectTracker.Application.Features.Issues.RemoveIssueAttachment;

namespace ProjectTracker.Api.Endpoints.Issues;

internal static class RemoveIssueAttachmentEndpoint
{
    public static void MapRemoveIssueAttachment(this IEndpointRouteBuilder app)
    {
        // DELETE /projects/{projectId}/issues/{issueId}/attachments/{attachmentId}
        app.MapDelete("/{issueId:guid}/attachments/{attachmentId:guid}", async (
            Guid projectId,
            Guid issueId,
            Guid attachmentId,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new RemoveIssueAttachmentCommand(projectId, issueId, attachmentId);

            await sender.Send(command, ct);

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
