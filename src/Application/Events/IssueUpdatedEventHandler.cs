using ProjectTracker.Domain.Events;

namespace ProjectTracker.Application.Events;

internal class IssueUpdatedEventHandler : INotificationHandler<IssueUpdatedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationMessageFactory _messageFactory;
    private readonly TimeProvider _timeProvider;

    public IssueUpdatedEventHandler(
        IApplicationDbContext context,
        INotificationMessageFactory messageFactory,
        TimeProvider timeProvider)
    {
        _context = context;
        _messageFactory = messageFactory;
        _timeProvider = timeProvider;
    }

    public async Task Handle(IssueUpdatedEvent domainEvent, CancellationToken ct)
    {
        var dto = await _context.Issues.AsNoTracking()
            .Where(i => i.Id == domainEvent.IssueId)
            .Select(i => new
            {
                ProjectKey = i.Project.Key,
                IssueKey = i.Key,
                IssueTitle = i.Title,
                WatcherIds = i.Watchers.Select(w => w.UserId),
                i.UpdatedBy
            })
            .FirstOrDefaultAsync(ct);

        if (dto is null) throw new EventHandlerException();

        var message = _messageFactory.IssueUpdated(dto.ProjectKey, dto.IssueKey, dto.IssueTitle);
        var now = _timeProvider.GetUtcNow();

        foreach (var watcherId in dto.WatcherIds)
        {
            if (watcherId == dto.UpdatedBy) continue;

            var notification = new Notification(watcherId, message, now);
            _context.Notifications.Add(notification);
        }

        await _context.SaveChangesAsync(ct);
    }
}
