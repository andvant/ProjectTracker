namespace ProjectTracker.Application.Features.Notifications.MarkRead;

public record MarkReadCommand : IRequest;

internal class MarkReadCommandHandler : IRequestHandler<MarkReadCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly TimeProvider _timeProvider;

    public MarkReadCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser,
        TimeProvider timeProvider)
    {
        _context = context;
        _currentUser = currentUser;
        _timeProvider = timeProvider;
    }

    public async Task Handle(MarkReadCommand command, CancellationToken ct)
    {
        var userId = _currentUser.GetUserId();

        var unreadNotifications = await _context.Notifications
            .Where(n => n.UserId == userId)
            .Where(n => n.ReadTime == null)
            .ToListAsync(ct);

        var now = _timeProvider.GetUtcNow();

        foreach (var notification in unreadNotifications)
        {
            notification.MarkRead(now);
        }

        await _context.SaveChangesAsync(ct);
    }
}
