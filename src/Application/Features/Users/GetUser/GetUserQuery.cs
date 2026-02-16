namespace ProjectTracker.Application.Features.Users.GetUser;

public record GetUserQuery(Guid Id) : IRequest<UserDto>;

internal class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
{
    private readonly IApplicationDbContext _context;

    public GetUserQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserDto> Handle(GetUserQuery query, CancellationToken ct)
    {
        return await _context.Users
            .Where(u => u.Id == query.Id)
            .ProjectToDto()
            .FirstOrDefaultAsync(ct) ?? throw new UserNotFoundException(query.Id);
    }
}
