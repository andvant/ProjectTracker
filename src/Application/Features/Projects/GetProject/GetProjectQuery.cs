using ProjectTracker.Application.Features.Projects.Common;

namespace ProjectTracker.Application.Features.Projects.GetProject;

public record GetProjectQuery(Guid Id) : IRequest<ProjectDto>;

public class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, ProjectDto>
{
    private readonly List<Project> _projects;
    private readonly ProjectDtoMapper _mapper;

    public GetProjectQueryHandler(
        List<Project> projects,
        ProjectDtoMapper mapper)
    {
        _projects = projects;
        _mapper = mapper;
    }

    public async Task<ProjectDto> Handle(GetProjectQuery query, CancellationToken cancellationToken)
    {
        var project = _projects.FirstOrDefault(p => p.Id == query.Id)
            ?? throw new ProjectNotFoundException(query.Id);

        return _mapper.ToDto(project);
    }
}
