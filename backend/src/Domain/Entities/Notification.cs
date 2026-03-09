namespace ProjectTracker.Domain.Entities;

public class Notification : Entity
{
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public DateTimeOffset Timestamp { get; private set; }
    public DateTimeOffset? ReadTime { get; private set; }
    public string Message { get; private set; }

    // for EF Core
    protected Notification()
    {
        User = null!;
        Message = null!;
    }

    public Notification(Guid userId, string message, DateTimeOffset timestamp)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new EmptyNotificationException();
        }

        User = null!;
        UserId = userId;
        Message = message;
        Timestamp = timestamp;
    }

    public void MarkRead(DateTimeOffset readTime)
    {
        ReadTime = readTime;
    }
}
