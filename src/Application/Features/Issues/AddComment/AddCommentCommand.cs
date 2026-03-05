namespace ProjectTracker.Application.Features.Issues.AddComment;

public record AddCommentCommand(
    Guid ProjectId,
    Guid IssueId,
    string Text,
    IssueStatus Status,
    Guid? AssigneeId) : IRequest;

internal class AddCommentCommandHandler : IRequestHandler<AddCommentCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<AddCommentCommandHandler> _logger;

    public AddCommentCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser,
        ILogger<AddCommentCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task Handle(AddCommentCommand command, CancellationToken ct)
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

        var userId = _currentUser.GetUserId();

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, ct)
            ?? throw new Exception($"Current user with id '{userId}' not found in the database");

        _currentUser.ValidateAllowed([.. issue.Project.Members.Select(m => m.UserId)]);

        User? assignee = null;

        if (command.AssigneeId.HasValue)
        {
            assignee = await _context.Users.FirstOrDefaultAsync(u => u.Id == command.AssigneeId.Value, ct)
                ?? throw new AssigneeNotFoundException(command.AssigneeId.Value);
        }

        issue.AddComment(user, command.Text, command.Status, assignee);

        _context.Update(issue);
        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Added comment to issue '{IssueId}' by user '{UserId}'",
            issue.Id, userId);
    }
}

public class AddCommentCommandValidator : AbstractValidator<AddCommentCommand>
{
    public AddCommentCommandValidator()
    {
        RuleFor(c => c.Text).NotEmpty();
        RuleFor(c => c.Status).IsInEnum();
    }
}
