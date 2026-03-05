using ProjectTracker.Application.Features.Projects.RemoveProjectAttachment;

namespace ProjectTracker.Api.Endpoints.Projects;

internal static class RemoveProjectAttachmentEndpoint
{
    public static void MapRemoveProjectAttachment(this IEndpointRouteBuilder app)
    {
        // DELETE /projects/{projectId}/attachments/{attachmentId}
        app.MapDelete("/{projectId:guid}/attachments/{attachmentId:guid}", async (
            Guid projectId,
            Guid attachmentId,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new RemoveProjectAttachmentCommand(projectId, attachmentId);

            await sender.Send(command, ct);

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
