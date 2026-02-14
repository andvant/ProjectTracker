using ProjectTracker.Application.Features.Projects.TransferOwnership;

namespace ProjectTracker.Api.Endpoints.Projects;

internal static class TransferOwnershipEndpoint
{
    public static void MapTransferOwnership(this IEndpointRouteBuilder app)
    {
        // PUT /projects/{projectId}/new-owner/{newOwnerId}
        app.MapPut("/{projectId:guid}/new-owner/{newOwnerId:guid}", async (
            Guid projectId,
            Guid newOwnerId,
            ISender sender,
            ILogger<Program> logger,
            CancellationToken ct) =>
        {
            var command = new TransferOwnershipCommand(projectId, newOwnerId);

            await sender.Send(command, ct);

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
