using MediatR;
using ProjectTracker.Features.Projects.Common;

namespace ProjectTracker.Features.Projects.GetProject;

public record GetProjectQuery(Guid Id) : IRequest<ProjectDto?>;

public class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, ProjectDto?>
{
    private readonly List<Project> _projects;
    private readonly ProjectDtoMapper _mapper;

    public GetProjectQueryHandler(
        [FromServices] List<Project> projects,
        ProjectDtoMapper mapper)
    {
        _projects = projects;
        _mapper = mapper;
    }

    public async Task<ProjectDto?> Handle(GetProjectQuery query, CancellationToken cancellationToken)
    {
        var project = _projects.FirstOrDefault(p => p.Id == query.Id);

        return project is null ? null : _mapper.ToDto(project);
    }
}
