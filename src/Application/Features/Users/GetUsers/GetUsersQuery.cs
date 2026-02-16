using Microsoft.EntityFrameworkCore;

namespace ProjectTracker.Application.Features.Users.GetUsers;

public record GetUsersQuery : IRequest<List<UsersDto>>;

internal class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UsersDto>>
{
    private readonly IApplicationDbContext _context;

    public GetUsersQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<UsersDto>> Handle(GetUsersQuery query, CancellationToken ct) =>
        _context.Users.ProjectToDto().ToListAsync(ct);
}
