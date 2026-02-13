using ProjectTracker.Domain.Enums;
using ProjectTracker.Domain.ValueObjects;

namespace ProjectTracker.Domain.Entities;

public class Issue : AuditableEntity
{
    public int Number { get; private set; }
    public IssueKey Key { get; private set; }
    public string Title { get; private set; }
    public string? Description { get; private set; }
    public Guid ReporterId { get; private set; }
    public User Reporter { get; private set; }
    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; }
    public Guid? AssigneeId { get; private set; }
    public User? Assignee { get; private set; }
    public IssueStatus Status { get; private set; }
    public IssueType Type { get; private set; }
    public IssuePriority Priority { get; private set; }

    public List<Attachment> Attachments { get; set; } = new();

    internal Issue(Project project, int number, string title,
        User reporter, User? assignee, IssueType? type, IssuePriority? priority)
    {
        Id = Guid.CreateVersion7();
        Number = number;
        Key = new IssueKey(project.Key, number);
        Title = title;
        Project = project;
        ProjectId = project.Id;
        Reporter = reporter;
        ReporterId = reporter.Id;
        Assignee = assignee;
        AssigneeId = assignee?.Id;
        Status = IssueStatus.Open;
        Type = type ?? IssueType.Task;
        Priority = priority ?? IssuePriority.Normal;
        CreatedOn = DateTime.UtcNow; // TODO: move to DbContext
    }

    public void UpdateDetails(string title, IssuePriority priority, string? description)
    {
        Title = title;
        Priority = priority;
        Description = description;
    }

    public void ChangeStatus(IssueStatus status)
    {
        Status = status;
    }
}
