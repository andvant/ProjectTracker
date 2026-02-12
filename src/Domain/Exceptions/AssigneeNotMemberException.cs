namespace ProjectTracker.Domain.Exceptions;

public class AssigneeNotMemberException : DomainException
{
    public Guid AssigneeId { get; }

    public AssigneeNotMemberException(Guid assigneeId)
        : base($"Assignee with id '{assigneeId}' is not a member of the project.")
    {
        AssigneeId = assigneeId;
    }
}
