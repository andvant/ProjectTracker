using ProjectTracker.Application.Features.Notifications.MarkRead;

namespace ProjectTracker.Api.Endpoints.Notifications;

internal static class MarkReadEndpoint
{
    public static void MapMarkRead(this IEndpointRouteBuilder app)
    {
        // GET /notifications/mark-read
        app.MapPost("/mark-read", async (ISender sender, CancellationToken ct) =>
        {
            var command = new MarkReadCommand();

            await sender.Send(command, ct);

            return Results.NoContent();
        })
        .Produces(StatusCodes.Status204NoContent);
    }
}
