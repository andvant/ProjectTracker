using Microsoft.Extensions.Logging;
using ProjectTracker.Application.Features.Projects.Common;

namespace ProjectTracker.Application.Features.Projects.CreateProject;

public record CreateProjectCommand(string Key, string Name, User User, string? Description) : IRequest<ProjectDto>;

internal class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly List<Project> _projects;
    private readonly ProjectDtoMapper _mapper;
    private readonly ILogger<CreateProjectCommandHandler> _logger;

    public CreateProjectCommandHandler(
        List<Project> projects,
        ProjectDtoMapper mapper,
        ILogger<CreateProjectCommandHandler> logger)
    {
        _projects = projects;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProjectDto> Handle(CreateProjectCommand command, CancellationToken ct)
    {
        var project = new Project(command.Key, command.Name, command.User, command.Description);

        _projects.Add(project);

        _logger.LogInformation(
            "Created project {Id} with key {Key}, name {Name}",
            project.Id, project.Key, project.Name);

        return _mapper.ToDto(project);
    }
}

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(c => c.Name).Must(Title.IsValid).WithMessage(Title.ValidationMessage);
        RuleFor(c => c.Key).Must(ProjectKey.IsValid).WithMessage(ProjectKey.ValidationMessage);
    }
}
