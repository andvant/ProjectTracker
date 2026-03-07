namespace ProjectTracker.Application.Exceptions;

public class EventHandlerException : ApplicationException
{
    public EventHandlerException() : base("Failed to handle domain event.")
    {
    }
}
