namespace ProjectTracker.Application.Interfaces.Caching;

public interface IAppCache
{
    Task<bool> Exists(string key, CancellationToken ct);
    Task<T?> TryGet<T>(string key, CancellationToken ct);
    Task Set<T>(string key, T value, TimeSpan? duration = null, CancellationToken ct = default);
    Task Remove(string key, CancellationToken ct);
}
