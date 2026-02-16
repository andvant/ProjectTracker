using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Issues.AddWatcher;

public record AddWatcherCommand(Guid ProjectId, Guid IssueId, Guid WatcherId) : IRequest;

internal class AddWatcherCommandHandler : IRequestHandler<AddWatcherCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<AddWatcherCommandHandler> _logger;

    public AddWatcherCommandHandler(
        IApplicationDbContext context,
        ILogger<AddWatcherCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(AddWatcherCommand command, CancellationToken ct)
    {
        var projectExists = await _context.Projects.AnyAsync(p => p.Id == command.ProjectId, ct);

        if (!projectExists)
        {
            throw new ProjectNotFoundException(command.ProjectId);
        }

        var issue = await _context.Projects
            .SelectMany(p => p.Issues)
            .Include(i => i.Project).ThenInclude(p => p.Members)
            .Include(i => i.Watchers)
            .FirstOrDefaultAsync(i => i.Id == command.IssueId, ct)
            ?? throw new IssueNotFoundException(command.IssueId);

        var watcher = await _context.Users.FirstOrDefaultAsync(u => u.Id == command.WatcherId, ct)
            ?? throw new WatcherNotFoundException(command.WatcherId);

        issue.AddWatcher(watcher);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Added watcher '{WatcherId}' to issue '{IssueId}'",
            watcher.Id, issue.Id);
    }
}
