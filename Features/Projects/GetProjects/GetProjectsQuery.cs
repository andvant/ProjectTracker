using MediatR;
using ProjectTracker.Features.Projects.Common;

namespace ProjectTracker.Features.Projects.GetProjects;

public record GetProjectsQuery : IRequest<List<ProjectsDto>>;

public class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, List<ProjectsDto>>
{
    private readonly List<Project> _projects;
    private readonly ProjectsDtoMapper _mapper;

    public GetProjectsQueryHandler(
        [FromServices] List<Project> projects,
        ProjectsDtoMapper mapper)
    {
        _projects = projects;
        _mapper = mapper;
    }

    public async Task<List<ProjectsDto>> Handle(GetProjectsQuery query, CancellationToken cancellationToken)
    {
        return _projects.Select(_mapper.ToDto).ToList();
    }
}
