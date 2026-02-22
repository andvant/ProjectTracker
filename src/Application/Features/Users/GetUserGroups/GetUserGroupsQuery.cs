namespace ProjectTracker.Application.Features.Users.GetUserGroups;

public record GetUserGroupsQuery(Guid UserId) : IRequest<IReadOnlyCollection<UserGroupDto>>;

internal class GetUserGroupsQueryHandler : IRequestHandler<GetUserGroupsQuery, IReadOnlyCollection<UserGroupDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IIdentityService _identityService;

    public GetUserGroupsQueryHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser,
        IIdentityService identityService)
    {
        _context = context;
        _currentUser = currentUser;
        _identityService = identityService;
    }

    public async Task<IReadOnlyCollection<UserGroupDto>> Handle(GetUserGroupsQuery query, CancellationToken ct)
    {
        _currentUser.ValidateAllowed();

        var userExists = await _context.Users.AnyAsync(u => u.Id == query.UserId);

        if (!userExists)
        {
            throw new UserNotFoundException(query.UserId);
        }

        var allGroups = await _identityService.GetAllGroups(ct);
        var userGroups = await _identityService.GetUserGroups(query.UserId, ct);

        return allGroups.Select(g => new UserGroupDto(
            g.Id,
            g.Name,
            g.Description,
            userGroups.Select(ug => ug.Id).Contains(g.Id)
        )).ToList();
    }
}
