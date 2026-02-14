using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Issues.UpdateIssue;

public record UpdateIssueCommand(
    Guid ProjectId,
    Guid IssueId,
    string Title,
    string? Description,
    Guid? AssigneeId,
    IssueStatus Status,
    IssuePriority Priority,
    DateTime? DueDate,
    int? EstimationMinutes) : IRequest;

internal class UpdateIssueCommandHandler : IRequestHandler<UpdateIssueCommand>
{
    private readonly List<Project> _projects;
    private readonly List<User> _users;
    private readonly ILogger<UpdateIssueCommandHandler> _logger;

    public UpdateIssueCommandHandler(
        List<Project> projects,
        List<User> users,
        ILogger<UpdateIssueCommandHandler> logger)
    {
        _projects = projects;
        _users = users;
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

        User? assignee = null;

        if (command.AssigneeId.HasValue)
        {
            assignee = _users.FirstOrDefault(u => u.Id == command.AssigneeId.Value)
                ?? throw new AssigneeNotFoundException(command.AssigneeId.Value);
        }

        issue.UpdateDetails(
            command.Title,
            command.Description,
            assignee,
            command.Status,
            command.Priority,
            command.DueDate,
            command.EstimationMinutes);

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
        RuleFor(c => c.DueDate).GreaterThanOrEqualTo(DateTime.UtcNow).When(c => c.DueDate.HasValue)
            .WithMessage("Due date must be in the future.");
        RuleFor(c => c.EstimationMinutes).GreaterThanOrEqualTo(0).When(c => c.EstimationMinutes.HasValue);
    }
}
