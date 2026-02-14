using ProjectTracker.Application.Features.Issues.AddWatcher;

namespace ProjectTracker.Api.Endpoints.Issues;

internal static class AddWatcherEndpoint
{
    public static void MapAddWatcher(this IEndpointRouteBuilder app)
    {
        // PUT /projects/{projectId}/issues/{issueId}/watchers/{watcherId}
        app.MapPut("/{issueId:guid}/watchers/{watcherId:guid}", async (
            Guid projectId,
            Guid issueId,
            Guid watcherId,
            ISender sender,
            ILogger<Program> logger,
            CancellationToken ct) =>
        {
            var command = new AddWatcherCommand(projectId, issueId, watcherId);

            await sender.Send(command, ct);

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
