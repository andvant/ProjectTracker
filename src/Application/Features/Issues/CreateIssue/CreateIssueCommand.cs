using Microsoft.Extensions.Logging;
using ProjectTracker.Application.Features.Issues.GetIssue;

namespace ProjectTracker.Application.Features.Issues.CreateIssue;

public record CreateIssueCommand(
    Guid ProjectId,
    string Title,
    string? Description,
    Guid? AssigneeId,
    IssueType? Type,
    IssuePriority? Priority,
    Guid? ParentIssueId,
    DateTimeOffset? DueDate,
    int? EstimationMinutes) : IRequest<IssueDto>;

internal class CreateIssueCommandHandler : IRequestHandler<CreateIssueCommand, IssueDto>
{
    private readonly IApplicationDbContext _context;
    private readonly User _currentUser;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<CreateIssueCommandHandler> _logger;

    public CreateIssueCommandHandler(
        IApplicationDbContext context,
        User currentUser,
        TimeProvider timeProvider,
        ILogger<CreateIssueCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task<IssueDto> Handle(CreateIssueCommand command, CancellationToken ct)
    {
        var reporter = await GetCurrentUser(ct);

        var project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == command.ProjectId, ct)
            ?? throw new ProjectNotFoundException(command.ProjectId);

        User? assignee = null;
        Issue? parentIssue = null;

        if (command.AssigneeId.HasValue)
        {
            assignee = await _context.Users.FirstOrDefaultAsync(u => u.Id == command.AssigneeId.Value, ct)
                ?? throw new AssigneeNotFoundException(command.AssigneeId.Value);
        }

        if (command.ParentIssueId.HasValue)
        {
            parentIssue = await _context.Projects
                .SelectMany(p => p.Issues)
                .FirstOrDefaultAsync(i => i.Id == command.ParentIssueId.Value, ct)
                ?? throw new ParentIssueNotFoundException(command.ParentIssueId.Value);
        }

        var nextIssueNumber = await GetNextIssueNumber(ct);

        var issue = project.CreateIssue(
            nextIssueNumber,
            command.Title,
            command.Description,
            reporter,
            assignee,
            command.Type,
            command.Priority,
            parentIssue,
            command.DueDate,
            _timeProvider.GetUtcNow(),
            command.EstimationMinutes);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Created issue with id '{Id}', key '{Key}', title '{Title}'",
            issue.Id, issue.Key, issue.Title);

        return issue.ToDto();
    }

    private async Task<User> GetCurrentUser(CancellationToken ct)
    {
        var currentUser = await _context.Users.FirstOrDefaultAsync(ct);

        if (currentUser is null)
        {
            _context.Users.Add(_currentUser);
            currentUser = _currentUser;
        }

        return currentUser;
    }

    private async Task<int> GetNextIssueNumber(CancellationToken ct)
    {
        var issues = _context.Projects.SelectMany(p => p.Issues);

        return (await issues.AnyAsync(ct))
            ? (await issues.MaxAsync(i => i.Number, ct)) + 1
            : 1;
    }
}

public class CreateIssueCommandValidator : AbstractValidator<CreateIssueCommand>
{
    public CreateIssueCommandValidator(TimeProvider timeProvider)
    {
        RuleFor(c => c.Title).Must(Title.IsValid).WithMessage(Title.ValidationMessage);
        RuleFor(c => c.Type).IsInEnum();
        RuleFor(c => c.Priority).IsInEnum();
        RuleFor(c => c.DueDate).GreaterThanOrEqualTo(timeProvider.GetUtcNow()).When(c => c.DueDate.HasValue)
            .WithMessage("Due date must be in the future.");
        RuleFor(c => c.EstimationMinutes).GreaterThanOrEqualTo(0).When(c => c.EstimationMinutes.HasValue);
    }
}
