using ProjectTracker.Domain.Events;

namespace ProjectTracker.Application.Events;

internal class MemberAddedEventHandler : INotificationHandler<MemberAddedEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationMessageFactory _messageFactory;
    private readonly TimeProvider _timeProvider;

    public MemberAddedEventHandler(
        IApplicationDbContext context,
        INotificationMessageFactory messageFactory,
        TimeProvider timeProvider)
    {
        _context = context;
        _messageFactory = messageFactory;
        _timeProvider = timeProvider;
    }

    public async Task Handle(MemberAddedEvent domainEvent, CancellationToken ct)
    {
        var message = _messageFactory.MemberAdded(domainEvent.Project.Key, domainEvent.Project.Name);

        var notification = new Notification(domainEvent.Member, message, _timeProvider.GetUtcNow());

        _context.Notifications.Add(notification);

        await _context.SaveChangesAsync(ct);
    }
}
