namespace ProjectTracker.Application.Interfaces;

public interface IObjectStorage
{
    Task<Stream> GetAsync(string storageKey, CancellationToken ct);
    Task<bool> UploadAsync(string storageKey, Stream stream, string mimeType, CancellationToken ct);
    Task<bool> DeleteAsync(string storageKey, CancellationToken ct);
    Task<IEnumerable<string>> ListAsync(CancellationToken ct);
}
