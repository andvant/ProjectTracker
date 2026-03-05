using ProjectTracker.Application.Features.Issues.DownloadIssueAttachment;

namespace ProjectTracker.Api.Endpoints.Issues;

internal static class DownloadIssueAttachmentEndpoint
{
    public static void MapDownloadIssueAttachment(this IEndpointRouteBuilder app)
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
            var query = new DownloadIssueAttachmentQuery(projectId, issueId, attachmentId);

            var file = await sender.Send(query, ct);

            httpContext.Response.Headers.ContentLength = file.Size;

            return TypedResults.File(file.Stream, file.MimeType, file.Name);
        })
        .AllowAnonymous() // TODO: remove and generate temp download link
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
