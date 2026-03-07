using ProjectTracker.Application.Features.Notifications.GetNotifications;

namespace ProjectTracker.Api.Endpoints.Notifications;

internal static class GetNotificationsEndpoint
{
    public static void MapGetNotifications(this IEndpointRouteBuilder app)
    {
        // GET /notifications
        app.MapGet("/", async (ISender sender, CancellationToken ct) =>
        {
            var query = new GetNotificationsQuery();

            var notifications = await sender.Send(query, ct);

            return TypedResults.Ok(notifications);
        })
        .Produces<IReadOnlyCollection<NotificationDto>>(StatusCodes.Status200OK);
    }
}
