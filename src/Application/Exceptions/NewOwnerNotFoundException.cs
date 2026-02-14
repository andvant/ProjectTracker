namespace ProjectTracker.Application.Exceptions;

public class NewOwnerNotFoundException : ApplicationException
{
    public Guid NewOwnerId { get; }

    public NewOwnerNotFoundException(Guid newOwnerId)
        : base($"New owner with id '{newOwnerId}' was not found.")
    {
        NewOwnerId = newOwnerId;
    }
}
