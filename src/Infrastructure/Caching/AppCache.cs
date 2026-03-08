using ProjectTracker.Application.Interfaces.Caching;
using ZiggyCreatures.Caching.Fusion;

namespace ProjectTracker.Infrastructure.Caching;

internal class AppCache : IAppCache
{
    private readonly IFusionCache _cache;

    public AppCache(IFusionCache cache)
    {
        _cache = cache;
    }

    public async Task<bool> Exists(string key, CancellationToken ct)
    {
        var value = await _cache.TryGetAsync<string>(key, token: ct);
        return value.HasValue;
    }

    public async Task<T?> TryGet<T>(string key, CancellationToken ct)
    {
        var value = await _cache.TryGetAsync<T>(key, token: ct);
        return value.HasValue ? value : default(T);
    }

    public async Task Set<T>(string key, T value, TimeSpan? duration = null, CancellationToken ct = default)
    {
        await _cache.SetAsync(key, value, new FusionCacheEntryOptions(duration), ct);
    }

    public async Task Remove(string key, CancellationToken ct)
    {
        await _cache.RemoveAsync(key, token: ct);
    }
}
