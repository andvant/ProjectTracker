namespace ProjectTracker.Application.Features.Users.GetUser;

public record GetUserQuery(Guid Id) : IRequest<UserDto>;

internal class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
{
    private readonly List<User> _users;
    private readonly UserDtoMapper _mapper;

    public GetUserQueryHandler(
        List<User> users,
        UserDtoMapper mapper)
    {
        _users = users;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(GetUserQuery query, CancellationToken ct)
    {
        var user = _users.FirstOrDefault(u => u.Id == query.Id)
            ?? throw new UserNotFoundException(query.Id);

        return _mapper.ToDto(user);
    }
}
