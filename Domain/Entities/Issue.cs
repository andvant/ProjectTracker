using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Domain.Entities;

public class Issue
{
    public required Guid Id { get; set; }
    public required int Number { get; set; }
    public required string ShortName { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public List<Attachment> Attachments { get; set; } = new();
    public required Guid CreatorId { get; set; }
    public required User Creator { get; set; }
    public required Guid ProjectId { get; set; }
    public required Project Project { get; set; }
    public required DateTime Created { get; set; }
    public required DateTime Updated { get; set; }
    public Guid? AssigneeId { get; set; }
    public User? Assignee { get; set; }
    public required IssueStatus Status { get; set; } = IssueStatus.Open;
    public required IssuePriority Priority { get; set; }

    [SetsRequiredMembers]
    public Issue(int number, string title, Project project,
        User creator, User? assignee = null, IssuePriority? priority = null)
    {
        Id = Guid.CreateVersion7();
        Number = number;
        ShortName = $"{project.ShortName}-{Number}";
        Title = title;
        Project = project;
        ProjectId = project.Id;
        Creator = creator;
        CreatorId = creator.Id;
        Assignee = assignee;
        AssigneeId = assignee?.Id;
        Priority = priority ?? IssuePriority.Normal;
        Created = DateTime.UtcNow;
        Updated = DateTime.UtcNow;
    }
}
