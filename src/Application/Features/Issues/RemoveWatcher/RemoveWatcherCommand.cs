using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Issues.RemoveWatcher;

public record RemoveWatcherCommand(Guid ProjectId, Guid IssueId, Guid WatcherId) : IRequest;

internal class RemoveWatcherCommandHandler : IRequestHandler<RemoveWatcherCommand>
{
    private readonly List<Project> _projects;
    private readonly List<User> _users;
    private readonly ILogger<RemoveWatcherCommandHandler> _logger;

    public RemoveWatcherCommandHandler(
        List<Project> projects,
        List<User> users,
        ILogger<RemoveWatcherCommandHandler> logger)
    {
        _projects = projects;
        _users = users;
        _logger = logger;
    }

    public async Task Handle(RemoveWatcherCommand command, CancellationToken ct)
    {
        var project = _projects.FirstOrDefault(p => p.Id == command.ProjectId);

        if (project is null)
        {
            throw new ProjectNotFoundException(command.ProjectId);
        }

        var issue = project.Issues.FirstOrDefault(i => i.Id == command.IssueId)
            ?? throw new IssueNotFoundException(command.IssueId);

        var watcher = _users.FirstOrDefault(u => u.Id == command.WatcherId)
            ?? throw new WatcherNotFoundException(command.WatcherId);

        issue.RemoveWatcher(watcher);

        _logger.LogInformation(
            "Removed watcher '{WatcherId}' from issue '{IssueId}'",
            watcher.Id, issue.Id);
    }
}
