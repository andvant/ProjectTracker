namespace ProjectTracker.Application.Features.Notifications.GetNotifications;

public record NotificationDto(
    Guid Id,
    string Message,
    DateTimeOffset Timestamp,
    DateTimeOffset? ReadTime
);
