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
    DateTimeOffset? DueDate,
    int? EstimationMinutes) : IRequest;

internal class UpdateIssueCommandHandler : IRequestHandler<UpdateIssueCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<UpdateIssueCommandHandler> _logger;

    public UpdateIssueCommandHandler(
        IApplicationDbContext context,
        TimeProvider timeProvider,
        ILogger<UpdateIssueCommandHandler> logger)
    {
        _context = context;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task Handle(UpdateIssueCommand command, CancellationToken ct)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == command.ProjectId, ct);

        if (project is null)
        {
            throw new ProjectNotFoundException(command.ProjectId);
        }

        var issue = project.Issues.FirstOrDefault(i => i.Id == command.IssueId)
            ?? throw new IssueNotFoundException(command.IssueId);

        User? assignee = null;

        if (command.AssigneeId.HasValue)
        {
            assignee = await _context.Users.FirstOrDefaultAsync(u => u.Id == command.AssigneeId.Value, ct)
                ?? throw new AssigneeNotFoundException(command.AssigneeId.Value);
        }

        issue.UpdateDetails(
            command.Title,
            command.Description,
            assignee,
            command.Status,
            command.Priority,
            command.DueDate,
            _timeProvider.GetUtcNow(),
            command.EstimationMinutes);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Updated project {Id} with key {Key}, name {Name}",
            project.Id, project.Key, project.Name);
    }
}

public class UpdateIssueCommandValidator : AbstractValidator<UpdateIssueCommand>
{
    public UpdateIssueCommandValidator(TimeProvider timeProvider)
    {
        RuleFor(c => c.Title).Must(Title.IsValid).WithMessage(Title.ValidationMessage);
        RuleFor(c => c.Status).IsInEnum();
        RuleFor(c => c.Priority).IsInEnum();
        RuleFor(c => c.DueDate).GreaterThanOrEqualTo(timeProvider.GetUtcNow()).When(c => c.DueDate.HasValue)
            .WithMessage("Due date must be in the future.");
        RuleFor(c => c.EstimationMinutes).GreaterThanOrEqualTo(0).When(c => c.EstimationMinutes.HasValue);
    }
}
