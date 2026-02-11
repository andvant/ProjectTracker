using Microsoft.Extensions.Logging;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Issues.UpdateIssue;

public record UpdateIssueCommand(Guid ProjectId, Guid IssueId, string Title,
    IssueStatus Status, IssuePriority Priority, string? Description) : IRequest;

public class UpdateIssueCommandHandler : IRequestHandler<UpdateIssueCommand>
{
    private readonly List<Project> _projects;
    private readonly ILogger<UpdateIssueCommandHandler> _logger;

    public UpdateIssueCommandHandler(
        List<Project> projects,
        ILogger<UpdateIssueCommandHandler> logger)
    {
        _projects = projects;
        _logger = logger;
    }

    public async Task Handle(UpdateIssueCommand command, CancellationToken cancellationToken)
    {
        var project = _projects.FirstOrDefault(p => p.Id == command.ProjectId);

        if (project is null)
        {
            throw new ApplicationException($"Project with id {command.ProjectId} not found");
        }

        var issue = project.Issues.FirstOrDefault(i => i.Id == command.IssueId);

        if (issue is null) return;

        issue.UpdateDetails(command.Title, command.Priority, command.Description);
        issue.ChangeStatus(command.Status);

        _logger.LogInformation(
            "Updated project {Id} with short name {ShortName}, name {Name}",
            project.Id, project.ShortName, project.Name);
    }
}

public class UpdateIssueCommandValidator : AbstractValidator<UpdateIssueCommand>
{
    public UpdateIssueCommandValidator()
    {
        RuleFor(c => c.Title).MaximumLength(100).NotEmpty();
        RuleFor(c => c.Status).IsInEnum();
        RuleFor(c => c.Priority).IsInEnum();
    }
}
