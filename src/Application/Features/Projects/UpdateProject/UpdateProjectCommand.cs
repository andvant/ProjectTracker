using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Projects.UpdateProject;

public record UpdateProjectCommand(Guid Id, string Name, string? Description) : IRequest;

internal class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand>
{
    private readonly List<Project> _projects;
    private readonly ILogger<UpdateProjectCommandHandler> _logger;

    public UpdateProjectCommandHandler(
        List<Project> projects,
        ILogger<UpdateProjectCommandHandler> logger)
    {
        _projects = projects;
        _logger = logger;
    }

    public async Task Handle(UpdateProjectCommand command, CancellationToken ct)
    {
        var project = _projects.FirstOrDefault(p => p.Id == command.Id)
            ?? throw new ProjectNotFoundException(command.Id);

        project.UpdateDetails(command.Name, command.Description);

        _logger.LogInformation(
            "Updated project {Id} with key {Key}, name {Name}",
            project.Id, project.Key, project.Name);
    }
}

public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(c => c.Name).MaximumLength(100).NotEmpty();
    }
}
