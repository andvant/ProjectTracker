using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Projects.UpdateProject;

public record UpdateProjectCommand(Guid Id, string Name, string? Description) : IRequest;

internal class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<UpdateProjectCommandHandler> _logger;

    public UpdateProjectCommandHandler(
        IApplicationDbContext context,
        ILogger<UpdateProjectCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(UpdateProjectCommand command, CancellationToken ct)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == command.Id, ct)
            ?? throw new ProjectNotFoundException(command.Id);

        project.UpdateDetails(command.Name, command.Description);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Updated project {Id} with key {Key}, name {Name}",
            project.Id, project.Key, project.Name);
    }
}

public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(c => c.Name).Must(Title.IsValid).WithMessage(Title.ValidationMessage);
    }
}
