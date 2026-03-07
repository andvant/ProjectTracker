namespace ProjectTracker.Application.Features.Notifications.GetNotifications;

public record GetNotificationsQuery : IRequest<IReadOnlyCollection<NotificationDto>>;

internal class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, IReadOnlyCollection<NotificationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetNotificationsQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyCollection<NotificationDto>> Handle(GetNotificationsQuery query, CancellationToken ct)
    {
        var userId = _currentUser.GetUserId();

        return await _context.Notifications.AsNoTracking()
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.Timestamp)
            .ProjectToDto()
            .ToListAsync(ct);
    }
}
