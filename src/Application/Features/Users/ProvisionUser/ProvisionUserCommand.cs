using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Users.ProvisionUser;

public record ProvisionUserCommand(Guid ExternalId, string Username, string Email) : IRequest;

internal class ProvisionUserCommandHandler : IRequestHandler<ProvisionUserCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<ProvisionUserCommandHandler> _logger;

    public ProvisionUserCommandHandler(
        IApplicationDbContext context,
        TimeProvider timeProvider,
        ILogger<ProvisionUserCommandHandler> logger)
    {
        _context = context;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task Handle(ProvisionUserCommand command, CancellationToken ct)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Id == command.ExternalId);

        if (userExists)
        {
            return;
        }

        var user = new User(command.ExternalId, command.Username, command.Email, _timeProvider.GetUtcNow());

        _context.Users.Add(user);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Created user with id '{Id}', username '{Username}', email '{Email}'",
            user.Id, user.Name, user.Email);
    }
}
