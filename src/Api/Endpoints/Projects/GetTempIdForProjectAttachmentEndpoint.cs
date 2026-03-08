using ProjectTracker.Application.Features.Projects.GetTempIdForProjectAttachment;

namespace ProjectTracker.Api.Endpoints.Projects;

internal static class GetTempIdForProjectAttachmentEndpoint
{
    public static void MapGetTempIdForProjectAttachment(this IEndpointRouteBuilder app)
    {
        // GET /projects/{projectId}/attachments/{attachmentId}
        app.MapGet("/{projectId:guid}/attachments/{attachmentId:guid}", async (
            Guid projectId,
            Guid attachmentId,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            var query = new GetTempIdForProjectAttachmentQuery(projectId, attachmentId);

            var tempId = await sender.Send(query, ct);

            return TypedResults.Ok(tempId);
        })
        .Produces(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);
    }
}
