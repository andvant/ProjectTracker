namespace ProjectTracker.Domain.Entities;

public class Issue : AuditableEntity
{
    public IssueKey Key { get; private set; }
    public int Number { get; private set; }
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
    public DateTimeOffset? DueDate { get; private set; }
    public int? EstimationMinutes { get; private set; }
    public Guid? ParentIssueId { get; private set; }
    public Issue? ParentIssue { get; private set; }

    public ICollection<User> Watchers { get; private set; } = new List<User>();
    public ICollection<Issue> ChildIssues { get; private set; } = new List<Issue>();
    public ICollection<Attachment> Attachments { get; private set; } = new List<Attachment>();

    protected Issue()
    {
        Key = null!;
        Title = null!;
        Reporter = null!;
        Project = null!;
    }

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
        ValidateDetails(project, reporter, assignee, dueDate, currentTime, estimationMinutes, parentIssue);

        Key = new IssueKey(project.Key, number);
        Number = number;
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

        if (reporter.Id != assignee?.Id)
        {
            Watchers.AddIfNotNull(assignee);
        }
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
        ValidateDetails(Project, null, assignee, dueDate, currentTime, estimationMinutes, null);

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

        Watchers.AddIfNotPresent(watcher);
    }

    public void RemoveWatcher(User watcher)
    {
        Watchers.RemoveIfPresent(watcher);
    }

    private void ValidateDetails(
        Project project,
        User? reporter,
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
        if (reporter is not null && !project.Members.Any(u => u.Id == reporter.Id))
        {
            throw new ReporterNotMemberException(reporter.Id);
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
