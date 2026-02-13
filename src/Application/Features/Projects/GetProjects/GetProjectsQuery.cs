namespace ProjectTracker.Application.Features.Projects.GetProjects;

public record GetProjectsQuery : IRequest<List<ProjectsDto>>;

internal class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, List<ProjectsDto>>
{
    private readonly List<Project> _projects;
    private readonly ProjectsDtoMapper _mapper;

    public GetProjectsQueryHandler(
        List<Project> projects,
        ProjectsDtoMapper mapper)
    {
        _projects = projects;
        _mapper = mapper;
    }

    public async Task<List<ProjectsDto>> Handle(GetProjectsQuery query, CancellationToken ct)
    {
        return _projects.Select(_mapper.ToDto).ToList();
    }
}
