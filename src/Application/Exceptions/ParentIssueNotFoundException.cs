namespace ProjectTracker.Application.Exceptions;

public class ParentIssueNotFoundException : ApplicationException
{
    public Guid ProjectId { get; }
    public Guid ParentIssueId { get; }

    public ParentIssueNotFoundException(Guid projectId, Guid parentIssueId)
        : base($"Parent issue with id '{parentIssueId}' in project with id '{projectId}' was not found.")
    {
        ProjectId = projectId;
        ParentIssueId = parentIssueId;
    }
}
