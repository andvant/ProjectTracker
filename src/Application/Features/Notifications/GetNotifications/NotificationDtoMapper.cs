using Riok.Mapperly.Abstractions;

namespace ProjectTracker.Application.Features.Notifications.GetNotifications;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class NotificationsDtoMapper
{
    public static partial NotificationDto ToDto(this Notification source);

    public static partial IQueryable<NotificationDto> ProjectToDto(this IQueryable<Notification> query);
}
