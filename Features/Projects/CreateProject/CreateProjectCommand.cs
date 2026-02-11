using MediatR;
using ProjectTracker.Features.Projects.Common;

namespace ProjectTracker.Features.Projects.CreateProject;

public record CreateProjectCommand(string ShortName, string Name, User User) : IRequest<ProjectDto>;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly List<Project> _projects;
    private readonly ProjectDtoMapper _mapper;
    private readonly ILogger<CreateProjectCommandHandler> _logger;

    public CreateProjectCommandHandler(
        [FromServices] List<Project> projects,
        ProjectDtoMapper mapper,
        ILogger<CreateProjectCommandHandler> logger)
    {
        _projects = projects;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProjectDto> Handle(CreateProjectCommand command, CancellationToken cancellationToken)
    {
        var project = new Project(command.ShortName, command.Name, command.User);

        _projects.Add(project);

        _logger.LogInformation(
            "Created project {Id} with short name {ShortName}, name {Name}",
            project.Id, project.ShortName, project.Name);

        return _mapper.ToDto(project);
    }
}

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(c => c.Name).MaximumLength(100).NotEmpty();
        RuleFor(c => c.ShortName).MaximumLength(10).Matches("^[a-zA-Z]{1}[a-zA-Z0-9]*$").NotEmpty();
    }
}
