namespace ProjectTracker.Application.Exceptions;

public class WatcherNotFoundException : ApplicationException
{
    public Guid WatcherId { get; }

    public WatcherNotFoundException(Guid watcherId)
        : base($"Watcher with id '{watcherId}' was not found.")
    {
        WatcherId = watcherId;
    }
}
