namespace ProjectTracker.Domain.Entities;

public class Issue : AuditableEntity
{
    public IssueKey Key { get; }
    public Title Title { get; private set; }
    public string? Description { get; private set; }
    public Guid ReporterId { get; }
    public User Reporter { get; }
    public Guid ProjectId { get; }
    public Project Project { get; }
    public Guid? AssigneeId { get; private set; }
    public User? Assignee { get; private set; }
    public IssueStatus Status { get; private set; }
    public IssueType Type { get; }
    public IssuePriority Priority { get; private set; }
    public DateTimeOffset? DueDate { get; private set; }
    public int? EstimationMinutes { get; private set; }
    public Guid? ParentIssueId { get; }
    public Issue? ParentIssue { get; }

    public ICollection<User> Watchers { get; } = new List<User>();
    public ICollection<Issue> ChildIssues { get; } = new List<Issue>();
    public ICollection<Attachment> Attachments { get; } = new List<Attachment>();

    internal Issue(
        Project project,
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
        ValidateDetails(project, assignee, dueDate, currentTime, estimationMinutes, parentIssue);

        Key = new IssueKey(project.Key, number);
        Title = title;
        Description = description;
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
        ParentIssue = parentIssue;
        ParentIssueId = parentIssue?.Id;
        parentIssue?.ChildIssues.Add(this);
        Watchers.Add(reporter);
        Watchers.AddIfNotNull(assignee);
    }

    public void UpdateDetails(
        string title,
        string? description,
        User? assignee,
        IssueStatus status,
        IssuePriority priority,
        DateTimeOffset? dueDate,
        DateTimeOffset currentTime,
        int? estimationMinutes)
    {
        ValidateDetails(Project, assignee, dueDate, currentTime, estimationMinutes, null);

        Title = title;
        Description = description;
        Assignee = assignee;
        AssigneeId = assignee?.Id;
        Status = status;
        Priority = priority;
        DueDate = dueDate;
        EstimationMinutes = estimationMinutes;
    }

    public void AddWatcher(User watcher)
    {
        if (!Project.Members.Any(u => u.Id == watcher.Id))
        {
            throw new WatcherNotMemberException(watcher.Id);
        }

        Watchers.AddIfNotThere(watcher);
    }

    public void RemoveWatcher(User watcher)
    {
        Watchers.RemoveIfExists(watcher);
    }

    private void ValidateDetails(
        Project project,
        User? assignee,
        DateTimeOffset? dueDate,
        DateTimeOffset currentTime,
        int? estimationMinutes,
        Issue? parentIssue)
    {
        if (assignee is not null && !project.Members.Any(u => u.Id == assignee.Id))
        {
            throw new AssigneeNotMemberException(assignee.Id);
        }
        if (dueDate.HasValue && dueDate < currentTime)
        {
            throw new PastDueDateException(dueDate.Value);
        }
        if (estimationMinutes.HasValue && estimationMinutes < 0)
        {
            throw new NegativeEstimationException(estimationMinutes.Value);
        }
        if (parentIssue != null)
        {
            ParentIssueWrongProjectException.ThrowIfMismatch(parentIssue.ProjectId, project.Id, parentIssue.Id);
            ParentIssueWrongTypeException.ThrowIfMismatch(parentIssue.Type, IssueType.Epic, parentIssue.Id);
        }
    }
}
