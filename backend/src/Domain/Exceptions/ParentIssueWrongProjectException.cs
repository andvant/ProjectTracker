namespace ProjectTracker.Domain.Exceptions;

public class ParentIssueWrongProjectException : DomainException
{
    public Guid ParentIssueId { get; }
    public Guid ActualProjectId { get; }
    public Guid ExpectedProjectId { get; }

    public ParentIssueWrongProjectException(Guid actualProjectId, Guid expectedProjectId, Guid parentIssueId)
        : base($"Parent issue '{parentIssueId}' belongs to a different project " +
            $"(expected: '{expectedProjectId}', actual: '{actualProjectId}').")
    {
        ParentIssueId = parentIssueId;
        ActualProjectId = actualProjectId;
        ExpectedProjectId = expectedProjectId;
    }

    internal static void ThrowIfMismatch(Guid actualProjectId, Guid expectedProjectId, Guid parentIssueId)
    {
        if (actualProjectId != expectedProjectId)
        {
            throw new ParentIssueWrongProjectException(actualProjectId, expectedProjectId, parentIssueId);
        }
    }
}
