using ProjectTracker.Application.Common;
using ProjectTracker.Application.Interfaces.Caching;

namespace ProjectTracker.Application.Features.Users.UpdateUserGroups;

public record UpdateUserGroupsCommand(Guid UserId, IReadOnlyCollection<Guid> GroupIds) : IRequest, ICacheInvalidator
{
    public string CacheKey => CacheKeys.UserGroups(UserId);
}

internal class UpdateUserGroupsCommandHandler : IRequestHandler<UpdateUserGroupsCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IIdentityService _identityService;
    private readonly ILogger<UpdateUserGroupsCommandHandler> _logger;

    public UpdateUserGroupsCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser,
        IIdentityService identityService,
        ILogger<UpdateUserGroupsCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
        _logger = logger;
    }

    public async Task Handle(UpdateUserGroupsCommand command, CancellationToken ct)
    {
        _currentUser.ValidateAllowed();

        var userExists = await _context.Users.AnyAsync(u => u.Id == command.UserId);

        if (!userExists)
        {
            throw new UserNotFoundException(command.UserId);
        }

        var userGroups = await _identityService.GetUserGroups(command.UserId, ct);

        var groupsToBeRemoved = userGroups.Where(ug => !command.GroupIds.Contains(ug.Id)).Select(g => g.Id).ToList();
        var groupsToBeAdded = command.GroupIds.Where(g => !userGroups.Select(ug => ug.Id).Contains(g)).ToList();

        foreach (var groupId in groupsToBeRemoved)
        {
            await _identityService.RemoveUserFromGroup(command.UserId, groupId, ct);
        }

        foreach (var groupId in groupsToBeAdded)
        {
            await _identityService.AddUserToGroup(command.UserId, groupId, ct);
        }

        _logger.LogInformation("Updated groups for user '{Id}'", command.UserId);
    }
}
