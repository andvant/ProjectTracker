namespace ProjectTracker.Domain.Entities;

public class Project : AuditableEntity
{
    public ProjectKey Key { get; private set; }
    public Title Name { get; private set; }
    public string? Description { get; private set; }
    public Guid OwnerId { get; private set; }
    public User Owner { get; private set; }

    public ICollection<ProjectMember> Members { get; private set; } = new List<ProjectMember>();
    public ICollection<Issue> Issues { get; private set; } = new List<Issue>();
    public ICollection<Attachment> Attachments { get; private set; } = new List<Attachment>();

    protected Project()
    {
        Key = null!;
        Name = null!;
        Owner = null!;
    }

    public Project(string key, string name, User owner, string? description, DateTimeOffset currentTime)
    {
        Key = key;
        Name = name;
        Owner = owner;
        OwnerId = owner.Id;
        Members.Add(new ProjectMember(this, owner, currentTime));
        Description = description;
    }

    public Issue CreateIssue(
        int number,
        string title,
        string? description,
        User reporter,
        User? assignee,
        IssueType? type,
        IssuePriority? priority,
        Issue? parentIssue,
        DateTimeOffset? dueDate,
        DateTimeOffset currentTime,
        int? estimationMinutes)
    {
        var issue = new Issue(
            this,
            number,
            title,
            description,
            reporter,
            assignee,
            type,
            priority,
            parentIssue,
            dueDate,
            currentTime,
            estimationMinutes);

        Issues.Add(issue);

        return issue;
    }

    public void RemoveIssue(Issue issue)
    {
        Issues.Remove(issue);
    }

    public void UpdateDetails(string name, string? description)
    {
        Name = name;
        Description = description;
    }

    public void AddMember(User member, DateTimeOffset currentTime)
    {
        if (!Members.Any(m => m.UserId == member.Id))
        {
            Members.Add(new ProjectMember(this, member, currentTime));
        }
    }

    public void RemoveMember(User member)
    {
        var existing = Members.FirstOrDefault(m => m.UserId == member.Id);

        if (existing is not null)
        {
            Members.Remove(existing);
        }
    }

    public void TransferOwnership(User newOwner)
    {
        if (!IsMember(newOwner))
        {
            throw new NewOwnerNotMemberException(newOwner.Id);
        }

        Owner = newOwner;
        OwnerId = newOwner.Id;
    }

    public bool IsMember(User user) =>
        Members.Any(u => u.UserId == user.Id);
}
