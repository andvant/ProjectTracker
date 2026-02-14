using Microsoft.Extensions.Logging;
using ProjectTracker.Application.Features.Issues.Common;

namespace ProjectTracker.Application.Features.Issues.CreateIssue;

public record CreateIssueCommand(
    Guid ProjectId,
    string Title,
    User Reporter,
    Guid? AssigneeId,
    IssueType? Type,
    IssuePriority? Priority,
    Guid? ParentIssueId,
    DateTime? DueDate,
    int? EstimationMinutes) : IRequest<IssueDto>;

internal class CreateIssueCommandHandler : IRequestHandler<CreateIssueCommand, IssueDto>
{
    private readonly List<Project> _projects;
    private readonly List<User> _users;
    private readonly IssueDtoMapper _mapper;
    private readonly ILogger<CreateIssueCommandHandler> _logger;

    public CreateIssueCommandHandler(
        List<Project> projects,
        List<User> users,
        IssueDtoMapper mapper,
        ILogger<CreateIssueCommandHandler> logger)
    {
        _projects = projects;
        _users = users;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<IssueDto> Handle(CreateIssueCommand command, CancellationToken ct)
    {
        var issues = _projects.SelectMany(p => p.Issues).ToList();
        var nextIssueNumber = issues.Any() ? issues.Max(i => i.Key.Number) + 1 : 1;

        var project = _projects.FirstOrDefault(p => p.Id == command.ProjectId);

        if (project is null)
        {
            throw new ProjectNotFoundException(command.ProjectId);
        }

        User? assignee = null;
        Issue? parentIssue = null;

        if (command.AssigneeId.HasValue)
        {
            assignee = _users.FirstOrDefault(u => u.Id == command.AssigneeId.Value)
                ?? throw new AssigneeNotFoundException(command.AssigneeId.Value);
        }

        if (command.ParentIssueId.HasValue)
        {
            parentIssue = _projects.SelectMany(p => p.Issues).FirstOrDefault(i => i.Id == command.ParentIssueId.Value)
                ?? throw new ParentIssueNotFoundException(command.ParentIssueId.Value);
        }

        var issue = project.CreateIssue(
            nextIssueNumber,
            command.Title,
            command.Reporter,
            assignee,
            command.Type,
            command.Priority,
            parentIssue,
            command.DueDate,
            command.EstimationMinutes);

        _logger.LogInformation(
            "Created issue with id '{Id}', key '{Key}', title '{Title}'",
            issue.Id, issue.Key, issue.Title);

        return _mapper.ToDto(issue);
    }
}

public class CreateIssueCommandValidator : AbstractValidator<CreateIssueCommand>
{
    public CreateIssueCommandValidator()
    {
        RuleFor(c => c.Title).Must(Title.IsValid).WithMessage(Title.ValidationMessage);
        RuleFor(c => c.Type).IsInEnum();
        RuleFor(c => c.Priority).IsInEnum();
        RuleFor(c => c.DueDate).GreaterThanOrEqualTo(DateTime.UtcNow).When(c => c.DueDate.HasValue)
            .WithMessage("Due date must be in the future.");
        RuleFor(c => c.EstimationMinutes).GreaterThanOrEqualTo(0).When(c => c.EstimationMinutes.HasValue);
    }
}
