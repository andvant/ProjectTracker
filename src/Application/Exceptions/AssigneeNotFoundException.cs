namespace ProjectTracker.Application.Exceptions;

public class AssigneeNotFoundException : ApplicationException
{
    public Guid AssigneeId { get; }

    public AssigneeNotFoundException(Guid assigneeId)
        : base($"Assignee with id '{assigneeId}' was not found.")
    {
        AssigneeId = assigneeId;
    }
}
