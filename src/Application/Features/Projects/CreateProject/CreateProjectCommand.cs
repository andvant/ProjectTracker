using Microsoft.Extensions.Logging;
using ProjectTracker.Application.Features.Projects.GetProject;

namespace ProjectTracker.Application.Features.Projects.CreateProject;

public record CreateProjectCommand(string Key, string Name, string? Description) : IRequest<ProjectDto>;

internal class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<CreateProjectCommandHandler> _logger;

    public CreateProjectCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser,
        TimeProvider timeProvider,
        ILogger<CreateProjectCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<ProjectDto> Handle(CreateProjectCommand command, CancellationToken ct)
    {
        await ValidateUniqueProjectKey(command.Key, ct);

        var userId = _currentUser.GetUserId();

        var owner = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new Exception($"Current user with id '{userId}' not found in the database");

        var project = new Project(command.Key, command.Name, owner, command.Description, _timeProvider.GetUtcNow());

        _context.Projects.Add(project);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Created project {Id} with key {Key}, name {Name}",
            project.Id, project.Key, project.Name);

        return project.ToDto();
    }

    private async Task ValidateUniqueProjectKey(string key, CancellationToken ct)
    {
        if (await _context.Projects.AnyAsync(p => p.Key == key, ct))
        {
            throw new ProjectKeyAlreadyExistsException(key);
        }
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
