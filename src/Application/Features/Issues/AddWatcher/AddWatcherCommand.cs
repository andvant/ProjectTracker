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
        var project = _context.Projects.FirstOrDefault(p => p.Id == command.ProjectId);

        if (project is null)
        {
            throw new ProjectNotFoundException(command.ProjectId);
        }

        var issue = project.Issues.FirstOrDefault(i => i.Id == command.IssueId)
            ?? throw new IssueNotFoundException(command.IssueId);

        var watcher = _context.Users.FirstOrDefault(u => u.Id == command.WatcherId)
            ?? throw new WatcherNotFoundException(command.WatcherId);

        issue.AddWatcher(watcher);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Added watcher '{WatcherId}' to issue '{IssueId}'",
            watcher.Id, issue.Id);
    }
}
