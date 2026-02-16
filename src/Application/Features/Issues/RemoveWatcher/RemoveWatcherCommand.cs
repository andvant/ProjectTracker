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
        var project = _context.Projects.FirstOrDefault(p => p.Id == command.ProjectId);

        if (project is null)
        {
            throw new ProjectNotFoundException(command.ProjectId);
        }

        var issue = project.Issues.FirstOrDefault(i => i.Id == command.IssueId)
            ?? throw new IssueNotFoundException(command.IssueId);

        var watcher = _context.Users.FirstOrDefault(u => u.Id == command.WatcherId)
            ?? throw new WatcherNotFoundException(command.WatcherId);

        issue.RemoveWatcher(watcher);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Removed watcher '{WatcherId}' from issue '{IssueId}'",
            watcher.Id, issue.Id);
    }
}
