namespace ProjectTracker.Domain.Exceptions;

public class ChildIssueWrongTypeException : DomainException
{
    public ChildIssueWrongTypeException()
        : base($"Child issue cannot be of type {IssueType.Epic}")
    {
    }
}
