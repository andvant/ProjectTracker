using System.Net.Mime;
using Microsoft.AspNetCore.StaticFiles;
using ProjectTracker.Application.Features.Projects.UploadProjectAttachment;

namespace ProjectTracker.Api.Endpoints.Projects;

internal static class UploadProjectAttachmentEndpoint
{
    public static void MapUploadProjectAttachment(this IEndpointRouteBuilder app)
    {
        // POST /projects/{projectId}/attachments
        app.MapPost("/{projectId:guid}/attachments", async (
            Guid projectId,
            IFormFile file,
            ISender sender,
            IContentTypeProvider contentTypeProvider,
            CancellationToken ct) =>
        {
            if (!contentTypeProvider.TryGetContentType(file.FileName, out var mimeType))
            {
                mimeType = MediaTypeNames.Application.Octet;
            }

            await using var stream = file.OpenReadStream();

            var command = new UploadProjectAttachmentCommand(
                projectId,
                Path.GetFileName(file.FileName),
                stream,
                mimeType);

            await sender.Send(command, ct);

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound)
        .ProducesValidationProblem()
        .DisableAntiforgery();
    }
}
