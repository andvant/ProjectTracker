namespace ProjectTracker.Application.Features.Users.GetUser;

public record GetUserQuery(Guid Id) : IRequest<UserDto>;

internal class GetUserQueryHandler : IRequestHandler<GetUserQuery, UserDto>
{
    private readonly IApplicationDbContext _context;
    private readonly UserDtoMapper _mapper;

    public GetUserQueryHandler(
        IApplicationDbContext context,
        UserDtoMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<UserDto> Handle(GetUserQuery query, CancellationToken ct)
    {
        var user = _context.Users.FirstOrDefault(u => u.Id == query.Id)
            ?? throw new UserNotFoundException(query.Id);

        return _mapper.ToDto(user);
    }
}
