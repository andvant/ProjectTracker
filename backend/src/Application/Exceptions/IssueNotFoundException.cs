namespace ProjectTracker.Application.Exceptions;

public class IssueNotFoundException : ApplicationException
{
    public Guid IssueId { get; }

    public IssueNotFoundException(Guid issueId)
        : base($"Issue with id '{issueId}' was not found.")
    {
        IssueId = issueId;
    }
}
