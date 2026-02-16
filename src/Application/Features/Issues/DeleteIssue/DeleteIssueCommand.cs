using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Issues.DeleteIssue;

public record DeleteIssueCommand(Guid ProjectId, Guid IssueId) : IRequest;

internal class DeleteIssueCommandHandler : IRequestHandler<DeleteIssueCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<DeleteIssueCommandHandler> _logger;

    public DeleteIssueCommandHandler(
        IApplicationDbContext context,
        ILogger<DeleteIssueCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(DeleteIssueCommand command, CancellationToken ct)
    {
        var project = await _context.Projects
            .Include(p => p.Issues)
            .FirstOrDefaultAsync(p => p.Id == command.ProjectId, ct)
            ?? throw new ProjectNotFoundException(command.ProjectId);

        var issue = project.Issues.FirstOrDefault(i => i.Id == command.IssueId)
            ?? throw new IssueNotFoundException(command.IssueId);

        project.RemoveIssue(issue);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Deleted issue with id '{Id}', key '{Key}', title '{Name}'",
            issue.Id, issue.Key, issue.Title);
    }
}
