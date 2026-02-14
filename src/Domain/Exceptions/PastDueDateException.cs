namespace ProjectTracker.Domain.Exceptions;

public class PastDueDateException : DomainException
{
    public DateTime DueDate { get; }

    public PastDueDateException(DateTime dueDate)
        : base($"Due date must be in the future. Value: '{dueDate}'")
    {
        DueDate = dueDate;
    }
}
