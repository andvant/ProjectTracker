using ProjectTracker.Domain.Events;

namespace ProjectTracker.Application.Events;

internal class NewOwnerEventHandler : INotificationHandler<NewOwnerEvent>
{
    private readonly IApplicationDbContext _context;
    private readonly INotificationMessageFactory _messageFactory;
    private readonly TimeProvider _timeProvider;

    public NewOwnerEventHandler(
        IApplicationDbContext context,
        INotificationMessageFactory messageFactory,
        TimeProvider timeProvider)
    {
        _context = context;
        _messageFactory = messageFactory;
        _timeProvider = timeProvider;
    }

    public async Task Handle(NewOwnerEvent domainEvent, CancellationToken ct)
    {
        var message = _messageFactory.NewOwner(domainEvent.ProjectKey, domainEvent.ProjectName);

        var notification = new Notification(domainEvent.NewOwnerId, message, _timeProvider.GetUtcNow());

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(ct);
    }
}
