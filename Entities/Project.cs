namespace ProjectTracker.Entities;

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

    public void AddIssue(Issue issue)
    {
        issue.ProjectId = Id;
        issue.Project = this;
        Issues.Add(issue);
    }
}
