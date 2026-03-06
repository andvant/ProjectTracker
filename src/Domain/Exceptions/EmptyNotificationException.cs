namespace ProjectTracker.Domain.Exceptions;

public class EmptyNotificationException : DomainException
{
    public EmptyNotificationException() : base("Notification message cannot be empty.")
    {
    }
}
