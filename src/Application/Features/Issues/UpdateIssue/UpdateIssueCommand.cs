using Microsoft.Extensions.Logging;
using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Issues.UpdateIssue;

public record UpdateIssueCommand(Guid ProjectId, Guid IssueId, string Title,
    IssueStatus Status, IssuePriority Priority, string? Description) : IRequest;

internal class UpdateIssueCommandHandler : IRequestHandler<UpdateIssueCommand>
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

    public async Task Handle(UpdateIssueCommand command, CancellationToken ct)
    {
        var project = _projects.FirstOrDefault(p => p.Id == command.ProjectId);

        if (project is null)
        {
            throw new ProjectNotFoundException(command.ProjectId);
        }

        var issue = project.Issues.FirstOrDefault(i => i.Id == command.IssueId)
            ?? throw new IssueNotFoundException(command.IssueId);

        issue.UpdateDetails(command.Title, command.Description, command.Status, command.Priority);

        _logger.LogInformation(
            "Updated project {Id} with key {Key}, name {Name}",
            project.Id, project.Key, project.Name);
    }
}

public class UpdateIssueCommandValidator : AbstractValidator<UpdateIssueCommand>
{
    public UpdateIssueCommandValidator()
    {
        RuleFor(c => c.Title).Must(Title.IsValid).WithMessage(Title.ValidationMessage);
        RuleFor(c => c.Status).IsInEnum();
        RuleFor(c => c.Priority).IsInEnum();
    }
}
