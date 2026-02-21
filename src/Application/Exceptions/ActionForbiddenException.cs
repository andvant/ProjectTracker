namespace ProjectTracker.Application.Exceptions;

public class ActionForbiddenException : ApplicationException
{
    public ActionForbiddenException()
        : base($"You're not allowed to perform this action.")
    {
    }
}
