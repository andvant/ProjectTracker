using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Projects.DeleteProject;

public record DeleteProjectCommand(Guid Id) : IRequest;

internal class DeleteProjectCommandHandler : IRequestHandler<DeleteProjectCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteProjectCommandHandler> _logger;

    public DeleteProjectCommandHandler(
        IApplicationDbContext context,
        ILogger<DeleteProjectCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(DeleteProjectCommand command, CancellationToken ct)
    {
        var project = _context.Projects.FirstOrDefault(p => p.Id == command.Id)
            ?? throw new ProjectNotFoundException(command.Id);

        _context.Projects.Remove(project);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Deleted project {Id} with key {Key}, name {Name}",
            project.Id, project.Key, project.Name);
    }
}
