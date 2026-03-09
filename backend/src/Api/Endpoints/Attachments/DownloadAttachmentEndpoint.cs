using ProjectTracker.Application.Features.Attachments.DownloadAttachment;

namespace ProjectTracker.Api.Endpoints.Attachments;

internal static class DownloadAttachmentEndpoint
{
    public static void MapDownloadAttachment(this IEndpointRouteBuilder app)
    {
        // GET /attachments/{tempId}
        app.MapGet("/{tempId:guid}", async (
            Guid tempId,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var query = new DownloadAttachmentQuery(tempId);

            var file = await sender.Send(query, ct);

            httpContext.Response.Headers.ContentLength = file.Size;

            return TypedResults.File(file.Stream, file.MimeType, file.Name);
        })
        .AllowAnonymous()
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
