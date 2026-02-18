namespace ProjectTracker.Domain.Entities;

public class Project : AuditableEntity
{
    public ProjectKey Key { get; private set; }
    public Title Name { get; private set; }
    public string? Description { get; private set; }
    public Guid OwnerId { get; private set; }
    public User Owner { get; private set; }

    public ICollection<ProjectMember> Members { get; private set; }
    public ICollection<Issue> Issues { get; private set; }
    public ICollection<ProjectAttachment> Attachments { get; private set; }

    // for EF Core
    protected Project()
    {
        Key = null!;
        Name = null!;
        Owner = null!;
        Members = null!;
        Issues = null!;
        Attachments = null!;
    }

    public Project(string key, string name, User owner, string? description, DateTimeOffset currentTime)
    {
        Key = key;
        Name = name;
        Owner = owner;
        OwnerId = owner.Id;
        Description = description;

        Members = new List<ProjectMember> { new(this, owner, currentTime) };
        Issues = new List<Issue>();
        Attachments = new List<ProjectAttachment>();
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

        Issues ??= new List<Issue>();
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

    public void AddAttachment(string name, string storageKey, string mimeType)
    {
        var attachment = new Attachment(name, storageKey, mimeType);

        Attachments ??= new List<ProjectAttachment>();
        Attachments.Add(new(this, attachment));
    }

    public void AddMember(User member, DateTimeOffset currentTime)
    {
        if (!IsMember(member))
        {
            Members.Add(new(this, member, currentTime));
        }
    }

    public void RemoveMember(User member)
    {
        if (OwnerId == member.Id)
        {
            throw new CantRemoveProjectOwnerException(member.Id);
        }

        var existing = Members.FirstOrDefault(m => m.UserId == member.Id);

        if (existing is not null)
        {
            foreach (var issue in Issues)
            {
                issue.RemoveAssignee(member);
                issue.RemoveWatcher(member);
            }

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
