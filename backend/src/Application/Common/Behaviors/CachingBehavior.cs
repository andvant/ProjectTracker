using ProjectTracker.Application.Interfaces.Caching;

namespace ProjectTracker.Application.Common.Behaviors;

internal class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : ICacheableQuery
{
    private readonly IAppCache _cache;

    public CachingBehavior(IAppCache cache)
    {
        _cache = cache;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        var cacheableQuery = request as ICacheableQuery;

        var cachedResponse = await _cache.TryGet<TResponse>(cacheableQuery.CacheKey, ct);

        if (cachedResponse is not null)
        {
            return cachedResponse;
        }

        var response = await next(ct);
        await _cache.Set(cacheableQuery.CacheKey, response, cacheableQuery.Duration, ct);

        return response;
    }
}
