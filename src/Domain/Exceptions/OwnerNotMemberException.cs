namespace ProjectTracker.Domain.Exceptions;

public class OwnerNotMemberException : DomainException
{
    public Guid NewOwnerId { get; }

    public OwnerNotMemberException(Guid newOwnerId)
        : base($"New owner with id '{newOwnerId}' is not a member of the project.")
    {
        NewOwnerId = newOwnerId;
    }
}
