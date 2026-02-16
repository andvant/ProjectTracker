namespace ProjectTracker.Domain.Exceptions;

public class CantRemoveProjectOwnerException : DomainException
{
    public Guid UserId { get; }

    public CantRemoveProjectOwnerException(Guid userId)
        : base($"Can't remove user with id '{userId}' from project members because they are the owner. " +
            $"Transfer ownership to another member first.")
    {
        UserId = userId;
    }
}
