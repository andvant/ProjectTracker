using ProjectTracker.Domain.Enums;
using ProjectTracker.Domain.ValueObjects;

namespace ProjectTracker.Domain.Entities;

public class Project : AuditableEntity
{
    public ProjectKey Key { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public Guid OwnerId { get; private set; }
    public User Owner { get; private set; }

    public List<Attachment> Attachments { get; private set; } = new();
    public List<User> Members { get; private set; } = new();
    public List<Issue> Issues { get; private set; } = new();

    public Project(string key, string name, User owner, string? description)
    {
        Id = Guid.CreateVersion7();
        Key = new ProjectKey(key);
        Name = name;
        Owner = owner;
        Members.Add(owner);
        Description = description;
        CreatedOn = DateTime.UtcNow; // TODO: move to DbContext
    }

    public Issue CreateIssue(
        int number,
        string title,
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
        if (!Members.Select(w => w.Id).Contains(member.Id))
        {
            Members.Add(member);
        }
    }

    public void RemoveMember(User member)
    {
        Members.RemoveAll(w => w.Id == member.Id);
    }

    public void TransferOwnership(User newOwner)
    {
        if (!Members.Select(w => w.Id).Contains(newOwner.Id))
        {
            throw new OwnerNotMemberException(newOwner.Id);
        }

        Owner = newOwner;
        OwnerId = newOwner.Id;
    }
}
