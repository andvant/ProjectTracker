namespace ProjectTracker.Domain.Entities;

public class IssueWatcher
{
    public Guid IssueId { get; private set; }
    public Issue Issue { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; }

    // for EF Core
    protected IssueWatcher()
    {
        Issue = null!;
        User = null!;
    }

    public IssueWatcher(Issue issue, User watcher)
    {
        Issue = issue;
        IssueId = issue.Id;
        User = watcher;
        UserId = watcher.Id;
    }
}
