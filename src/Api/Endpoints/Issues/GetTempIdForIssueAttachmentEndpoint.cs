using ProjectTracker.Application.Features.Issues.GetTempIdForIssueAttachment;

namespace ProjectTracker.Api.Endpoints.Issues;

internal static class GetTempIdForIssueAttachmentEndpoint
{
    public static void MapGetTempIdForIssueAttachment(this IEndpointRouteBuilder app)
    {
        // GET /projects/{projectId}/issues/{issueId}/attachments/{attachmentId}
        app.MapGet("/{issueId:guid}/attachments/{attachmentId:guid}", async (
            Guid projectId,
            Guid issueId,
            Guid attachmentId,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var query = new GetTempIdForIssueAttachmentQuery(projectId, issueId, attachmentId);

            var tempId = await sender.Send(query, ct);

            return TypedResults.Ok(tempId);
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
