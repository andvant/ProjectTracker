namespace ProjectTracker.Application.Interfaces;

public interface IObjectStorage
{
    Task<Stream> GetStreamAsync(string storageKey, CancellationToken ct);
    Task<string> UploadAsync(string storageKey, Stream stream, string mimeType, CancellationToken ct);
    Task<bool> DeleteAsync(string storageKey, CancellationToken ct);
    Task<IEnumerable<string>> ListAsync(CancellationToken ct);
}
