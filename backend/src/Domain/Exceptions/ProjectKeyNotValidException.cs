namespace ProjectTracker.Domain.Exceptions;

public class ProjectKeyNotValidException : DomainException
{
    public string ProjectKey { get; }

    public ProjectKeyNotValidException(string projectKey, string message)
        : base(message)
    {
        ProjectKey = projectKey;
    }
}
