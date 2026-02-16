using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProjectTracker.Application.Features.Projects.GetProject;

namespace ProjectTracker.Application.Features.Projects.CreateProject;

public record CreateProjectCommand(string Key, string Name, string? Description) : IRequest<ProjectDto>;

internal class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IApplicationDbContext _context;
    private readonly User _currentUser;
    private readonly ProjectDtoMapper _mapper;
    private readonly ILogger<CreateProjectCommandHandler> _logger;

    public CreateProjectCommandHandler(
        IApplicationDbContext context,
        User currentUser,
        ProjectDtoMapper mapper,
        ILogger<CreateProjectCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProjectDto> Handle(CreateProjectCommand command, CancellationToken ct)
    {
        await ValidateUniqueProjectKey(command.Key);

        var owner = await GetCurrentUser();

        var project = new Project(command.Key, command.Name, owner, command.Description);

        _context.Projects.Add(project);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Created project {Id} with key {Key}, name {Name}",
            project.Id, project.Key, project.Name);

        return _mapper.ToDto(project);
    }

    private async Task ValidateUniqueProjectKey(string key)
    {
        if (await _context.Projects.AnyAsync(p => p.Key == key))
        {
            throw new ProjectKeyAlreadyExistsException(key);
        }
    }

    private async Task<User> GetCurrentUser()
    {
        var currentUser = await _context.Users.FirstOrDefaultAsync();

        if (currentUser is null)
        {
            _context.Users.Add(_currentUser);
            currentUser = _currentUser;
        }

        return currentUser;
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
