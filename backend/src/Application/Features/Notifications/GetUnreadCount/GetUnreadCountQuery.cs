namespace ProjectTracker.Application.Features.Notifications.GetUnreadCount;

public record GetUnreadCountQuery : IRequest<int>;

internal class GetUnreadCountQueryHandler : IRequestHandler<GetUnreadCountQuery, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetUnreadCountQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(GetUnreadCountQuery query, CancellationToken ct)
    {
        var userId = _currentUser.GetUserId();

        return await _context.Notifications.AsNoTracking()
            .Where(n => n.UserId == userId)
            .Where(n => n.ReadTime == null)
            .CountAsync(ct);
    }
}
