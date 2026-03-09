namespace ProjectTracker.Application.Exceptions;

public class UserNotFoundException : ApplicationException
{
    public Guid UserId { get; }

    public UserNotFoundException(Guid userId)
        : base($"User with id '{userId}' was not found.")
    {
        UserId = userId;
    }
}
