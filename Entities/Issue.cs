using ProjectTracker.Enums;

namespace ProjectTracker.Entities;

public class Issue
{
    public required Guid Id { get; set; }
    public required int Number { get; set; }
    public required string ShortName { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public List<Attachment> Attachments { get; set; } = new();
    public required Guid CreatorId { get; set; }
    public required User Creator { get; set; }
    public required Guid ProjectId { get; set; }
    public required Project Project { get; set; }
    public Guid? AssigneeId { get; set; }
    public User? Assignee { get; set; }
    public required IssueStatus Status { get; set; } = IssueStatus.Open;
    public required IssuePriority Priority { get; set; } = IssuePriority.Normal;

    public Issue(string title, Project project, User creator, User? assignee)
    {
        Title = title;
        Project = project;
        ProjectId = project.Id;
        Creator = creator;
        CreatorId = creator.Id;
        Assignee = assignee;
        AssigneeId = assignee?.Id;
    }
}
