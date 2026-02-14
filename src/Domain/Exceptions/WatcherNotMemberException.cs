namespace ProjectTracker.Domain.Exceptions;

public class WatcherNotMemberException : DomainException
{
    public Guid WatcherId { get; }

    public WatcherNotMemberException(Guid watcherId)
        : base($"Watcher with id '{watcherId}' is not a member of the project.")
    {
        WatcherId = watcherId;
    }
}
