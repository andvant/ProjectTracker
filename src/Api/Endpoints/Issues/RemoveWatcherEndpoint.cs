using ProjectTracker.Application.Features.Issues.RemoveWatcher;

namespace ProjectTracker.Api.Endpoints.Issues;

internal static class RemoveWatcherEndpoint
{
    public static void MapRemoveWatcher(this IEndpointRouteBuilder app)
    {
        // DELETE /projects/{projectId}/issues/{issueId}/watchers/{watcherId}
        app.MapDelete("/{issueId:guid}/watchers/{watcherId:guid}", async (
            Guid projectId,
            Guid issueId,
            Guid watcherId,
            ISender sender,
            ILogger<Program> logger,
            CancellationToken ct) =>
        {
            var command = new RemoveWatcherCommand(projectId, issueId, watcherId);

            await sender.Send(command, ct);

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
