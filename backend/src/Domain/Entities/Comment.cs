namespace ProjectTracker.Domain.Entities;

public class Comment : AuditableEntity
{
    public Guid IssueId { get; private set; }
    public Issue Issue { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; }

    public string Text { get; private set; }

    // for EF Core
    protected Comment()
    {
        User = null!;
        Issue = null!;
        Text = null!;
    }

    public Comment(Issue issue, User user, string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new EmptyCommentException();
        }

        Issue = issue;
        IssueId = issue.Id;
        User = user;
        UserId = user.Id;
        Text = text;
    }
}
