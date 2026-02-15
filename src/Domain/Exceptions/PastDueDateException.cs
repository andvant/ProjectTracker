namespace ProjectTracker.Domain.Exceptions;

public class PastDueDateException : DomainException
{
    public DateTimeOffset DueDate { get; }

    public PastDueDateException(DateTimeOffset dueDate)
        : base($"Due date must be in the future. Value: '{dueDate}'")
    {
        DueDate = dueDate;
    }
}
