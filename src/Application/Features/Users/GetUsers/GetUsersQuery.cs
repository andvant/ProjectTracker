namespace ProjectTracker.Application.Features.Users.GetUsers;

public record GetUsersQuery : IRequest<IReadOnlyCollection<UsersDto>>;

internal class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, IReadOnlyCollection<UsersDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<UsersDto>> Handle(GetUsersQuery query, CancellationToken ct) =>
        await _context.Users.AsNoTracking().ProjectToDto().ToListAsync(ct);
}
