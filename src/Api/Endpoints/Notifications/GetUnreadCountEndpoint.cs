using ProjectTracker.Application.Features.Notifications.GetUnreadCount;

namespace ProjectTracker.Api.Endpoints.Notifications;

internal static class GetUnreadCountEndpoint
{
    public static void MapGetUnreadCount(this IEndpointRouteBuilder app)
    {
        // GET /notifications/unread-count
        app.MapGet("/unread-count", async (ISender sender, CancellationToken ct) =>
        {
            var query = new GetUnreadCountQuery();

            var unreadCount = await sender.Send(query, ct);

            return TypedResults.Ok(unreadCount);
        })
        .Produces<int>(StatusCodes.Status200OK);
    }
}
