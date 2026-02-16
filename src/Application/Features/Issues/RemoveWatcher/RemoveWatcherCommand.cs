using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Issues.RemoveWatcher;

public record RemoveWatcherCommand(Guid ProjectId, Guid IssueId, Guid WatcherId) : IRequest;

internal class RemoveWatcherCommandHandler : IRequestHandler<RemoveWatcherCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<RemoveWatcherCommandHandler> _logger;

    public RemoveWatcherCommandHandler(
        IApplicationDbContext context,
        ILogger<RemoveWatcherCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(RemoveWatcherCommand command, CancellationToken ct)
    {
        var projectExists = await _context.Projects.AnyAsync(p => p.Id == command.ProjectId, ct);

        if (!projectExists)
        {
            throw new ProjectNotFoundException(command.ProjectId);
        }

        var issue = await _context.Projects
            .Where(p => p.Id == command.ProjectId)
            .SelectMany(p => p.Issues)
            .Include(i => i.Watchers)
            .FirstOrDefaultAsync(i => i.Id == command.IssueId, ct)
            ?? throw new IssueNotFoundException(command.IssueId);

        var watcher = await _context.Users.FirstOrDefaultAsync(u => u.Id == command.WatcherId, ct)
            ?? throw new WatcherNotFoundException(command.WatcherId);

        issue.RemoveWatcher(watcher);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Removed watcher '{WatcherId}' from issue '{IssueId}'",
            watcher.Id, issue.Id);
    }
}
