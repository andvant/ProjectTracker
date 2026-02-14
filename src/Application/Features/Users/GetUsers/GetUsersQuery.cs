namespace ProjectTracker.Application.Features.Users.GetUsers;

public record GetUsersQuery : IRequest<List<UsersDto>>;

internal class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, List<UsersDto>>
{
    private readonly List<User> _users;
    private readonly UsersDtoMapper _mapper;

    public GetUsersQueryHandler(
        List<User> users,
        UsersDtoMapper mapper)
    {
        _users = users;
        _mapper = mapper;
    }

    public async Task<List<UsersDto>> Handle(GetUsersQuery query, CancellationToken ct)
    {
        return _users.Select(_mapper.ToDto).ToList();
    }
}
