using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Projects.UpdateProject;

public record UpdateProjectCommand(Guid Id, string Name, string? Description) : IRequest;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand>
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

    public async Task Handle(UpdateProjectCommand command, CancellationToken cancellationToken)
    {
        var project = _projects.FirstOrDefault(p => p.Id == command.Id);

        if (project is null) return;

        project.Name = command.Name;
        project.Description = command.Description;

        _logger.LogInformation(
            "Updated project {Id} with short name {ShortName}, name {Name}",
            project.Id, project.ShortName, project.Name);
    }
}

public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(c => c.Name).MaximumLength(100).NotEmpty();
    }
}
