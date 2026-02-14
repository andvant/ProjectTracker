using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Issues.AddWatcher;

public record AddWatcherCommand(Guid ProjectId, Guid IssueId, Guid WatcherId) : IRequest;

internal class AddWatcherCommandHandler : IRequestHandler<AddWatcherCommand>
{
    private readonly List<Project> _projects;
    private readonly List<User> _users;
    private readonly ILogger<AddWatcherCommandHandler> _logger;

    public AddWatcherCommandHandler(
        List<Project> projects,
        List<User> users,
        ILogger<AddWatcherCommandHandler> logger)
    {
        _projects = projects;
        _users = users;
        _logger = logger;
    }

    public async Task Handle(AddWatcherCommand command, CancellationToken ct)
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

        issue.AddWatcher(watcher);

        _logger.LogInformation(
            "Added watcher '{WatcherId}' to issue '{IssueId}'",
            watcher.Id, issue.Id);
    }
}
