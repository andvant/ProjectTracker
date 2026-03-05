using ProjectTracker.Application.Features.Projects.DownloadProjectAttachment;

namespace ProjectTracker.Api.Endpoints.Projects;

internal static class DownloadProjectAttachmentEndpoint
{
    public static void MapDownloadProjectAttachment(this IEndpointRouteBuilder app)
    {
        // GET /projects/{projectId}/attachments/{attachmentId}
        app.MapGet("/{projectId:guid}/attachments/{attachmentId:guid}", async (
            Guid projectId,
            Guid attachmentId,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var query = new DownloadProjectAttachmentQuery(projectId, attachmentId);

            var file = await sender.Send(query, ct);

            httpContext.Response.Headers.ContentLength = file.Size;

            return TypedResults.File(file.Stream, file.MimeType, file.Name);
        })
        .AllowAnonymous() // TODO: remove and generate temp download link
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
