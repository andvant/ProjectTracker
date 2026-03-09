namespace ProjectTracker.Domain.Exceptions;

public class PastDueDateException : DomainException
{
    public DateOnly DueDate { get; }

    public PastDueDateException(DateOnly dueDate)
        : base($"Due date must be in the future. Value: '{dueDate}'")
    {
        DueDate = dueDate;
    }
}
