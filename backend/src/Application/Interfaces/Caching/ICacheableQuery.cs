namespace ProjectTracker.Application.Interfaces.Caching;

public interface ICacheableQuery
{
    string CacheKey { get; }
    TimeSpan? Duration { get; }
}
