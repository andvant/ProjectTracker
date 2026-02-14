using ProjectTracker.Application.Features.Projects.RemoveMember;

namespace ProjectTracker.Api.Endpoints.Projects;

internal static class RemoveMemberEndpoint
{
    public static void MapRemoveMember(this IEndpointRouteBuilder app)
    {
        // DELETE /projects/{projectId}/members/{memberId}
        app.MapDelete("/{projectId:guid}/members/{memberId:guid}", async (
            Guid projectId,
            Guid memberId,
            ISender sender,
            ILogger<Program> logger,
            CancellationToken ct) =>
        {
            var command = new RemoveMemberCommand(projectId, memberId);

            await sender.Send(command, ct);

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
