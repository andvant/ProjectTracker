using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Issues.RemoveIssueAttachment;

public record RemoveIssueAttachmentCommand(Guid ProjectId, Guid IssueId, Guid AttachmentId) : IRequest;

internal class RemoveIssueAttachmentCommandHandler : IRequestHandler<RemoveIssueAttachmentCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<RemoveIssueAttachmentCommandHandler> _logger;

    public RemoveIssueAttachmentCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser,
        ILogger<RemoveIssueAttachmentCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task Handle(RemoveIssueAttachmentCommand command, CancellationToken ct)
    {
        var projectOwnerId = await _context.Projects.Where(p => p.Id == command.ProjectId)
            .Select(p => p.OwnerId).FirstOrDefaultAsync(ct);

        if (projectOwnerId == Guid.Empty)
        {
            throw new ProjectNotFoundException(command.ProjectId);
        }

        var issue = await _context.Projects
            .Where(p => p.Id == command.ProjectId)
            .SelectMany(p => p.Issues)
            .Include(i => i.Attachments)
            .FirstOrDefaultAsync(i => i.Id == command.IssueId, ct)
            ?? throw new IssueNotFoundException(command.IssueId);

        var attachment = await _context.Attachments.FirstOrDefaultAsync(a => a.Id == command.AttachmentId, ct)
            ?? throw new AttachmentNotFoundException(command.AttachmentId);

        _currentUser.ValidateAllowed([issue.ReporterId, projectOwnerId]);

        issue.RemoveAttachment(attachment);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Removed attachment '{AttachmentId}' from issue '{IssueId}'",
            attachment.Id, issue.Id);
    }
}
