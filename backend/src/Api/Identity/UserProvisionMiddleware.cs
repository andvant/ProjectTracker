using System.Collections.Concurrent;
using System.Security.Claims;
using ProjectTracker.Application.Common;
using ProjectTracker.Application.Features.Users.ProvisionUser;
using ProjectTracker.Application.Interfaces.Caching;

namespace ProjectTracker.Api.Identity;

public class UserProvisionMiddleware : IMiddleware
{
    private readonly ISender _sender;
    private readonly IAppCache _cache;
    private static readonly ConcurrentDictionary<Guid, SemaphoreSlim> _locks = new();

    public UserProvisionMiddleware(ISender sender, IAppCache cache)
    {
        _sender = sender;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var user = context.User;

        if (user.Identity?.IsAuthenticated != true)
        {
            await next(context);
            return;
        }

        var ct = context.RequestAborted;
        var id = Guid.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (await _cache.Exists(CacheKeys.UserExists(id), ct))
        {
            await next(context);
            return;
        }

        var semaphore = _locks.GetOrAdd(id, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(ct);

        try
        {
            var email = user.FindFirstValue(ClaimTypes.Email)!;
            var username = user.FindFirstValue("preferred_username")!;
            var fullName = user.FindFirstValue("name")!;

            var command = new ProvisionUserCommand(id, username, email, fullName);

            await _sender.Send(command, ct);

            await _cache.Set(CacheKeys.UserExists(id), "1", TimeSpan.FromHours(2), ct);

            await next(context);
        }
        finally
        {
            semaphore.Release();
        }
    }
}
