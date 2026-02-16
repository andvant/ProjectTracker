namespace ProjectTracker.Application.Features.Users.GetUsers;

public record GetUsersQuery : IRequest<List<UsersDto>>;

internal class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UsersDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly UsersDtoMapper _mapper;

    public GetUsersQueryHandler(
        IApplicationDbContext context,
        UsersDtoMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<UsersDto>> Handle(GetUsersQuery query, CancellationToken ct)
    {
        return _context.Users.Select(_mapper.ToDto).ToList();
    }
}
