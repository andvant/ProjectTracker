using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Projects.RemoveProjectAttachment;

public record RemoveProjectAttachmentCommand(Guid ProjectId, Guid AttachmentId) : IRequest;

internal class RemoveProjectAttachmentCommandHandler : IRequestHandler<RemoveProjectAttachmentCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<RemoveProjectAttachmentCommandHandler> _logger;

    public RemoveProjectAttachmentCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser,
        ILogger<RemoveProjectAttachmentCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task Handle(RemoveProjectAttachmentCommand command, CancellationToken ct)
    {
        var project = await _context.Projects
            .Include(p => p.Attachments)
            .FirstOrDefaultAsync(p => p.Id == command.ProjectId, ct)
            ?? throw new ProjectNotFoundException(command.ProjectId);

        _currentUser.ValidateAllowed([project.OwnerId]);

        var attachment = await _context.Attachments.FirstOrDefaultAsync(a => a.Id == command.AttachmentId, ct)
            ?? throw new AttachmentNotFoundException(command.AttachmentId);

        project.RemoveAttachment(attachment);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Removed attachment '{AttachmentId}' from project '{ProjectId}'",
            attachment.Id, project.Id);
    }
}
