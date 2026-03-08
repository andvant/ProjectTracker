namespace ProjectTracker.Application.Interfaces;

public interface IAppCache
{
    Task<bool> Exists(string key, CancellationToken ct);
    Task Set<T>(string key, T value, TimeSpan? duration = null, CancellationToken ct = default);
}
