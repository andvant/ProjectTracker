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

    public ICollection<IssueWatcher> Watchers { get; private set; }
    public IReadOnlyCollection<Issue> ChildIssues { get; private set; }
    public ICollection<IssueAttachment> Attachments { get; private set; }

    // for EF Core
    protected Issue()
    {
        Key = null!;
        Title = null!;
        Reporter = null!;
        Project = null!;
        Watchers = null!;
        ChildIssues = null!;
        Attachments = null!;
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
        ValidateDetails(project, reporter, assignee, dueDate, currentTime, estimationMinutes, type, parentIssue);

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

        ChildIssues = new List<Issue>();
        Attachments = new List<IssueAttachment>();
        Watchers = new List<IssueWatcher>() { new(this, reporter) };

        if (assignee is not null && reporter.Id != assignee.Id)
        {
            Watchers.Add(new(this, assignee));
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
        ValidateDetails(Project, null, assignee, dueDate, currentTime, estimationMinutes, null, null);

        Title = title;
        Description = description;
        Assignee = assignee;
        AssigneeId = assignee?.Id;
        Status = status;
        Priority = priority;
        DueDate = dueDate;
        EstimationMinutes = estimationMinutes;
    }

    public void AddAttachment(string name, string storageKey, string mimeType)
    {
        var attachment = new Attachment(name, storageKey, mimeType);

        Attachments ??= new List<IssueAttachment>();
        Attachments.Add(new(this, attachment));
    }

    public void AddWatcher(User watcher)
    {
        if (!Project.IsMember(watcher))
        {
            throw new WatcherNotMemberException(watcher.Id);
        }

        if (!Watchers.Any(w => w.UserId == watcher.Id))
        {
            Watchers.Add(new(this, watcher));
        }
    }

    public void RemoveWatcher(User watcher)
    {
        var existing = Watchers.FirstOrDefault(w => w.UserId == watcher.Id);

        if (existing is not null)
        {
            Watchers.Remove(existing);
        }
    }

    internal void RemoveAssignee(User assignee)
    {
        if (AssigneeId == assignee.Id)
        {
            Assignee = null;
            AssigneeId = null;
        }
    }

    private void ValidateDetails(
        Project project,
        User? reporter,
        User? assignee,
        DateTimeOffset? dueDate,
        DateTimeOffset currentTime,
        int? estimationMinutes,
        IssueType? type,
        Issue? parentIssue)
    {
        if (assignee is not null && !project.IsMember(assignee))
        {
            throw new AssigneeNotMemberException(assignee.Id);
        }
        if (reporter is not null && !project.IsMember(reporter))
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
        if (parentIssue is not null)
        {
            ParentIssueWrongProjectException.ThrowIfMismatch(parentIssue.ProjectId, project.Id, parentIssue.Id);
            ParentIssueWrongTypeException.ThrowIfMismatch(parentIssue.Type, IssueType.Epic, parentIssue.Id);

            if (type.HasValue && type.Value == IssueType.Epic)
            {
                throw new ChildIssueWrongTypeException();
            }
        }
    }
}
