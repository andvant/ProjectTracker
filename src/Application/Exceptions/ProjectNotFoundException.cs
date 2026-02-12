namespace ProjectTracker.Application.Exceptions;

public class ProjectNotFoundException : ApplicationException
{
    public Guid ProjectId { get; }

    public ProjectNotFoundException(Guid projectId)
        : base($"Project with id '{projectId}' was not found.")
    {
        ProjectId = projectId;
    }
}
