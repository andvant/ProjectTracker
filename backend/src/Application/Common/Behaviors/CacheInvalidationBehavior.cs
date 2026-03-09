using ProjectTracker.Application.Interfaces.Caching;

namespace ProjectTracker.Application.Common.Behaviors;

internal class CacheInvalidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IAppCache _cache;

    public CacheInvalidationBehavior(IAppCache cache)
    {
        _cache = cache;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var response = await next(ct);

        if (request is ICacheInvalidator cacheInvalidator)
        {
            await _cache.Remove(cacheInvalidator.CacheKey, ct);
        }

        return response;
    }
}
