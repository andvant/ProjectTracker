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
    private readonly ICurrentUser _currentUser;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<UpdateIssueCommandHandler> _logger;

    public UpdateIssueCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser,
        TimeProvider timeProvider,
        ILogger<UpdateIssueCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task Handle(UpdateIssueCommand command, CancellationToken ct)
    {
        var projectExists = await _context.Projects.AnyAsync(p => p.Id == command.ProjectId, ct);

        if (!projectExists)
        {
            throw new ProjectNotFoundException(command.ProjectId);
        }

        var issue = await _context.Projects
            .Where(p => p.Id == command.ProjectId)
            .SelectMany(p => p.Issues)
            .Include(i => i.Project).ThenInclude(p => p.Members)
            .FirstOrDefaultAsync(i => i.Id == command.IssueId, ct)
            ?? throw new IssueNotFoundException(command.IssueId);

        _currentUser.ValidateAllowed([issue.ReporterId, issue.Project.OwnerId]);

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
            "Updated issue '{Id}' with key '{Key}', title '{Title}'",
            issue.Id, issue.Key, issue.Title);
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
