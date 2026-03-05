namespace ProjectTracker.Application.Features.Common;

public record FileDto(string Name, long Size, string MimeType, Stream Stream);
