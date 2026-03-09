namespace ProjectTracker.Domain.Exceptions;

public class IssueNumberNotValidException : DomainException
{
    public int Number { get; }

    public IssueNumberNotValidException(int number)
        : base($"'{number}' is not a valid number for an issue.")
    {
        Number = number;
    }
}
