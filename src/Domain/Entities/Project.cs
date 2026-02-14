namespace ProjectTracker.Domain.Entities;

public class Project : AuditableEntity
{
    public ProjectKey Key { get; private set; }
    public Title Name { get; private set; }
    public string? Description { get; private set; }
    public Guid OwnerId { get; private set; }
    public User Owner { get; private set; }

    public ICollection<Attachment> Attachments { get; private set; } = new List<Attachment>();
    public ICollection<User> Members { get; private set; } = new List<User>();
    public ICollection<Issue> Issues { get; private set; } = new List<Issue>();

    public Project(string key, string name, User owner, string? description)
    {
        Id = Guid.CreateVersion7();
        Key = key;
        Name = name;
        Owner = owner;
        OwnerId = owner.Id;
        Members.Add(owner);
        Description = description;
        CreatedOn = DateTime.UtcNow; // TODO: move to DbContext
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
        DateTime? dueDate,
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

    public void AddMember(User member)
    {
        Members.AddIfNotThere(member);
    }

    public void RemoveMember(User member)
    {
        Members.RemoveIfExists(member);
    }

    public void TransferOwnership(User newOwner)
    {
        if (!Members.Any(u => u.Id == newOwner.Id))
        {
            throw new NewOwnerNotMemberException(newOwner.Id);
        }

        Owner = newOwner;
        OwnerId = newOwner.Id;
    }
}
