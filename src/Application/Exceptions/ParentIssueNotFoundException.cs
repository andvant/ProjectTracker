namespace ProjectTracker.Application.Exceptions;

public class ParentIssueNotFoundException : ApplicationException
{
    public Guid ParentIssueId { get; }

    public ParentIssueNotFoundException(Guid parentIssueId)
        : base($"Parent issue with id '{parentIssueId}' was not found.")
    {
        ParentIssueId = parentIssueId;
    }
}
