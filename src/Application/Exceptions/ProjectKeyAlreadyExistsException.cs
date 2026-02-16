namespace ProjectTracker.Application.Exceptions;

public class ProjectKeyAlreadyExistsException : ApplicationException
{
    public string Key { get; }

    public ProjectKeyAlreadyExistsException(string key)
        : base($"Project with key '{key}' already exists.")
    {
        Key = key;
    }
}
