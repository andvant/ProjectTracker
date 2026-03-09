namespace ProjectTracker.Domain.Exceptions;

public class ParentIssueWrongTypeException : DomainException
{
    public Guid ParentIssueId { get; }
    public IssueType ActualType { get; }
    public IssueType ExpectedType { get; }

    public ParentIssueWrongTypeException(IssueType actualType, IssueType expectedType, Guid parentIssueId)
        : base($"Parent issue '{parentIssueId}' is of type '{actualType}', but '{expectedType}' was expected.")
    {
        ParentIssueId = parentIssueId;
        ActualType = actualType;
        ExpectedType = expectedType;
    }

    public static void ThrowIfMismatch(IssueType actualType, IssueType expectedType, Guid parentIssueId)
    {
        if (actualType != expectedType)
        {
            throw new ParentIssueWrongTypeException(actualType, expectedType, parentIssueId);
        }
    }
}
