namespace ProjectTracker.Domain.Exceptions;

public class NewOwnerNotMemberException : DomainException
{
    public Guid NewOwnerId { get; }

    public NewOwnerNotMemberException(Guid newOwnerId)
        : base($"New owner with id '{newOwnerId}' is not a member of the project.")
    {
        NewOwnerId = newOwnerId;
    }
}
