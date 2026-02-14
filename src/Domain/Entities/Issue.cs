using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Domain.Entities;

public class Issue : AuditableEntity
{
    public IssueKey Key { get; private set; }
    public Title Title { get; private set; }
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
    public DateTime? DueDate { get; private set; }
    public int? EstimationMinutes { get; private set; }
    public Guid? ParentIssueId { get; private set; }
    public Issue? ParentIssue { get; private set; }

    public List<Issue> ChildIssues { get; private set; } = new();
    public List<Attachment> Attachments { get; set; } = new();
    public List<User> Watchers { get; private set; } = new();

    internal Issue(
        Project project,
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
        ValidateDetails(project, assignee, dueDate, estimationMinutes, parentIssue);

        Id = Guid.CreateVersion7();
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
        DueDate = dueDate;
        EstimationMinutes = estimationMinutes;
        CreatedOn = DateTime.UtcNow; // TODO: move to DbContext

        if (parentIssue != null)
        {
            ParentIssue = parentIssue;
            ParentIssueId = parentIssue.Id;
            parentIssue.ChildIssues.Add(this);
        }
    }

    public void UpdateDetails(string title, string? description, IssueStatus status, IssuePriority priority)
    {
        Title = title;
        Description = description;
        Status = status;
        Priority = priority;
    }

    public void AddWatcher(User watcher)
    {
        if (!Watchers.Select(w => w.Id).Contains(watcher.Id))
        {
            Watchers.Add(watcher);
        }
    }

    public void RemoveWatcher(User watcher)
    {
        Watchers.RemoveAll(w => w.Id == watcher.Id);
    }

    private void ValidateDetails(
        Project project,
        User? assignee,
        DateTime? dueDate,
        int? estimationMinutes,
        Issue? parentIssue)
    {
        if (assignee is not null && !project.Members.Select(u => u.Id).Contains(assignee.Id))
        {
            throw new AssigneeNotMemberException(assignee.Id);
        }
        if (dueDate.HasValue && dueDate < DateTime.UtcNow) // TODO: inject TimeProvider
        {
            throw new PastDueDateException(dueDate.Value);
        }
        if (estimationMinutes.HasValue && estimationMinutes < 0)
        {
            throw new NegativeEstimationException(estimationMinutes.Value);
        }
        if (parentIssue != null)
        {
            ParentIssueWrongProjectException.ThrowIfMismatch(parentIssue.ProjectId, ProjectId, parentIssue.Id);
            ParentIssueWrongTypeException.ThrowIfMismatch(parentIssue.Type, IssueType.Epic, parentIssue.Id);
        }
    }
}
