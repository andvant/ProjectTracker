using ProjectTracker.Domain.Events;

namespace ProjectTracker.Application.Events;

internal class IssueAssignedEventHandler : INotificationHandler<IssueAssignedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationMessageFactory _messageFactory;
    private readonly TimeProvider _timeProvider;

    public IssueAssignedEventHandler(
        IApplicationDbContext context,
        INotificationMessageFactory messageFactory,
        TimeProvider timeProvider)
    {
        _context = context;
        _messageFactory = messageFactory;
        _timeProvider = timeProvider;
    }

    public async Task Handle(IssueAssignedEvent domainEvent, CancellationToken ct)
    {
        var dto = await _context.Issues.AsNoTracking()
            .Where(i => i.Id == domainEvent.IssueId)
            .Select(i => new
            {
                ProjectKey = i.Project.Key,
                IssueKey = i.Key,
                IssueTitle = i.Title,
                i.UpdatedBy
            })
            .FirstOrDefaultAsync(ct);

        if (dto is null) throw new EventHandlerException();

        if (dto.UpdatedBy == domainEvent.AssigneeId)
        {
            return;
        }

        var message = _messageFactory.IssueAssigned(dto.ProjectKey, dto.IssueKey, dto.IssueTitle);

        var notification = new Notification(domainEvent.AssigneeId, message, _timeProvider.GetUtcNow());

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(ct);
    }
}
