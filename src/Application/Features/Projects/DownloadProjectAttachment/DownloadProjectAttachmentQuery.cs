using ProjectTracker.Application.Features.Common;

namespace ProjectTracker.Application.Features.Projects.DownloadProjectAttachment;

public record DownloadProjectAttachmentQuery(Guid ProjectId, Guid AttachmentId) : IRequest<FileDto>;

internal class DownloadProjectAttachmentQueryHandler : IRequestHandler<DownloadProjectAttachmentQuery, FileDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IObjectStorage _storage;
    private readonly ICurrentUser _currentUser;

    public DownloadProjectAttachmentQueryHandler(
        IApplicationDbContext context,
        IObjectStorage storage,
        ICurrentUser currentUser)
    {
        _context = context;
        _storage = storage;
        _currentUser = currentUser;
    }

    public async Task<FileDto> Handle(DownloadProjectAttachmentQuery query, CancellationToken ct)
    {
        var projectExists = await _context.Projects.AnyAsync(p => p.Id == query.ProjectId, ct);

        if (!projectExists)
        {
            throw new ProjectNotFoundException(query.ProjectId);
        }

        var memberIds = await _context.GetProjectMemberIds(query.ProjectId, ct);

        //_currentUser.ValidateAllowed(memberIds, [Roles.ProjectManager]); // TODO: uncomment after using temp download link

        var attachment = await _context.Projects
            .Where(p => p.Id == query.ProjectId)
            .SelectMany(p => p.Attachments)
            .Where(a => a.AttachmentId == query.AttachmentId)
            .Select(a => a.Attachment)
            .FirstOrDefaultAsync(ct);

        if (attachment is null)
        {
            throw new AttachmentNotFoundException(query.AttachmentId);
        }

        var file = await _storage.GetAsync(attachment.StorageKey, ct);

        return new FileDto(attachment.Name, file.Length, attachment.MimeType, file);
    }
}
