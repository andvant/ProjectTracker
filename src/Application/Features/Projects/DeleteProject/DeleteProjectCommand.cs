using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Projects.DeleteProject;

public record DeleteProjectCommand(Guid Id) : IRequest;

internal class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand>
{
    private readonly List<Project> _projects;
    private readonly ILogger<DeleteProjectCommandHandler> _logger;

    public DeleteProjectCommandHandler(
        List<Project> projects,
        ILogger<DeleteProjectCommandHandler> logger)
    {
        _projects = projects;
        _logger = logger;
    }

    public async Task Handle(DeleteProjectCommand command, CancellationToken cancellationToken)
    {
        var project = _projects.FirstOrDefault(p => p.Id == command.Id)
            ?? throw new ProjectNotFoundException(command.Id);

        _projects.Remove(project);

        _logger.LogInformation(
            "Deleted project {Id} with short name {ShortName}, name {Name}",
            project.Id, project.ShortName, project.Name);
    }
}
