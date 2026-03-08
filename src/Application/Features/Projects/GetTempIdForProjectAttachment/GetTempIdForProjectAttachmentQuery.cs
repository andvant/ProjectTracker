using ProjectTracker.Application.Common;
using ProjectTracker.Application.Features.Attachments.DownloadAttachment;
using ProjectTracker.Application.Interfaces.Caching;

namespace ProjectTracker.Application.Features.Projects.GetTempIdForProjectAttachment;

public record GetTempIdForProjectAttachmentQuery(Guid ProjectId, Guid AttachmentId) : IRequest<Guid>;

internal class GetTempIdForProjectAttachmentQueryHandler : IRequestHandler<GetTempIdForProjectAttachmentQuery, Guid>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IAppCache _cache;

    public GetTempIdForProjectAttachmentQueryHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser,
        IAppCache cache)
    {
        _context = context;
        _currentUser = currentUser;
        _cache = cache;
    }

    public async Task<Guid> Handle(GetTempIdForProjectAttachmentQuery query, CancellationToken ct)
    {
        var projectExists = await _context.Projects.AnyAsync(p => p.Id == query.ProjectId, ct);

        if (!projectExists)
        {
            throw new ProjectNotFoundException(query.ProjectId);
        }

        var memberIds = await _context.GetProjectMemberIds(query.ProjectId, ct);

        _currentUser.ValidateAllowed(memberIds, [Roles.ProjectManager]);

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

        var downloadDto = new AttachmentDownloadDto(attachment.StorageKey, attachment.Name, attachment.MimeType);

        var tempId = Guid.NewGuid();

        await _cache.Set(CacheKeys.AttachmentDownload(tempId), downloadDto, TimeSpan.FromMinutes(5), ct);

        return tempId;
    }
}
