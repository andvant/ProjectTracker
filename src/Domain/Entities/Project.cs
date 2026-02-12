using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Domain.Entities;

public class Project
{
    public required Guid Id { get; set; }
    public required string ShortName { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required DateTime Created { get; set; }
    public List<Attachment> Attachments { get; set; } = new();
    public required User Owner { get; set; }
    public List<User> Members { get; set; } = new();
    public List<Issue> Issues { get; set; } = new();

    [SetsRequiredMembers]
    public Project(string shortName, string name, User owner)
    {
        Id = Guid.CreateVersion7();
        ShortName = shortName;
        Name = name;
        Owner = owner;
        Members.Add(owner);
        Created = DateTime.UtcNow;
    }

    public Issue CreateIssue(int number, string title, User creator,
        User? assignee = null, IssuePriority? priority = null)
    {
        if (assignee is not null && !Members.Select(u => u.Id).Contains(assignee.Id))
        {
            throw new AssigneeNotMemberException(assignee.Id);
        }

        var issue = new Issue(this, number, title, creator, assignee, priority);
        Issues.Add(issue);

        return issue;
    }

    public void RemoveIssue(Issue issue)
    {
        Issues.Remove(issue);
    }
}
