using ProjectTracker.Application.Common;
using ProjectTracker.Application.Interfaces.Caching;

namespace ProjectTracker.Application.Features.Users.ProvisionUser;

public record ProvisionUserCommand(Guid Id, string Username, string Email, string FullName) : IRequest;

internal class ProvisionUserCommandHandler : IRequestHandler<ProvisionUserCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IAppCache _cache;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<ProvisionUserCommandHandler> _logger;

    public ProvisionUserCommandHandler(
        IApplicationDbContext context,
        IAppCache cache,
        TimeProvider timeProvider,
        ILogger<ProvisionUserCommandHandler> logger)
    {
        _context = context;
        _cache = cache;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task Handle(ProvisionUserCommand command, CancellationToken ct)
    {
        if (await _cache.Exists(CacheKeys.UserExists(command.Id), ct))
        {
            return;
        }

        var userExists = await _context.Users.AnyAsync(u => u.Id == command.Id, ct);

        if (userExists)
        {
            await CacheUser(command.Id, ct);
            return;
        }

        var user = new User(command.Id, command.Username, command.Email, command.FullName, _timeProvider.GetUtcNow());

        _context.Users.Add(user);

        await _context.SaveChangesAsync(ct);

        await CacheUser(command.Id, ct);

        _logger.LogInformation(
            "Created user with id '{Id}', username '{Username}', email '{Email}'",
            user.Id, user.Username, user.Email);
    }

    private Task CacheUser(Guid UserId, CancellationToken ct) =>
        _cache.Set(CacheKeys.UserExists(UserId), "1", TimeSpan.FromHours(2), ct);
}
