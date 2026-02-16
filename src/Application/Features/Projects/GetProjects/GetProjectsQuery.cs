namespace ProjectTracker.Application.Features.Projects.GetProjects;

public record GetProjectsQuery : IRequest<List<ProjectsDto>>;

internal class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, List<ProjectsDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ProjectsDtoMapper _mapper;

    public GetProjectsQueryHandler(
        IApplicationDbContext context,
        ProjectsDtoMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<ProjectsDto>> Handle(GetProjectsQuery query, CancellationToken ct)
    {
        return _context.Projects.Select(_mapper.ToDto).ToList();
    }
}
