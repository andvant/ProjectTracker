using System.Net.Mime;
using Microsoft.AspNetCore.StaticFiles;
using ProjectTracker.Application.Features.Issues.UploadIssueAttachment;

namespace ProjectTracker.Api.Endpoints.Issues;

internal static class UploadIssueAttachmentEndpoint
{
    public static void MapUploadIssueAttachment(this IEndpointRouteBuilder app)
    {
        // POST /projects/{projectId}/issues/{issueId}/attachments
        app.MapPost("/{issueId:guid}/attachments", async (
            Guid projectId,
            Guid issueId,
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

            var command = new UploadIssueAttachmentCommand(
                projectId,
                issueId,
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
