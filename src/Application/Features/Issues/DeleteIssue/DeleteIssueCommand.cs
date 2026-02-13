using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Issues.DeleteIssue;

public record DeleteIssueCommand(Guid ProjectId, Guid IssueId) : IRequest;

internal class DeleteIssueCommandHandler : IRequestHandler<DeleteIssueCommand>
{
    private readonly List<Project> _projects;
    private readonly ILogger<DeleteIssueCommandHandler> _logger;

    public DeleteIssueCommandHandler(
        List<Project> projects,
        ILogger<DeleteIssueCommandHandler> logger)
    {
        _projects = projects;
        _logger = logger;
    }

    public async Task Handle(DeleteIssueCommand command, CancellationToken ct)
    {
        var project = _projects.FirstOrDefault(p => p.Id == command.ProjectId);

        if (project is null)
        {
            throw new ProjectNotFoundException(command.ProjectId);
        }

        var issue = project.Issues.FirstOrDefault(i => i.Id == command.IssueId)
            ?? throw new IssueNotFoundException(command.IssueId);

        project.RemoveIssue(issue);

        _logger.LogInformation(
            "Deleted issue with id '{Id}', short name '{ShortName}', title '{Name}'",
            issue.Id, issue.ShortName, issue.Title);
    }
}
