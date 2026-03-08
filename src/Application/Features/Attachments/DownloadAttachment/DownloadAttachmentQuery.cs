using ProjectTracker.Application.Common;
using ProjectTracker.Application.Features.Common;
using ProjectTracker.Application.Interfaces.Caching;

namespace ProjectTracker.Application.Features.Attachments.DownloadAttachment;

public record DownloadAttachmentQuery(Guid TempId) : IRequest<FileDto>;

internal class DownloadAttachmentQueryHandler : IRequestHandler<DownloadAttachmentQuery, FileDto>
{
    private readonly IObjectStorage _storage;
    private readonly IAppCache _cache;

    public DownloadAttachmentQueryHandler(IObjectStorage storage, IAppCache cache)
    {
        _storage = storage;
        _cache = cache;
    }

    public async Task<FileDto> Handle(DownloadAttachmentQuery query, CancellationToken ct)
    {
        var downloadDto = await _cache.TryGet<AttachmentDownloadDto>(CacheKeys.AttachmentDownload(query.TempId), ct);

        if (downloadDto is null)
        {
            throw new DownloadTempIdNotFoundException(query.TempId);
        }

        await _cache.Remove(CacheKeys.AttachmentDownload(query.TempId), ct);

        var file = await _storage.GetAsync(downloadDto.StorageKey, ct);

        return new FileDto(downloadDto.Name, file.Length, downloadDto.MimeType, file);
    }
}
