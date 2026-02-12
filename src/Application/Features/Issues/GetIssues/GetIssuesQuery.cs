namespace ProjectTracker.Application.Features.Issues.GetIssues;

public record GetIssuesQuery(Guid ProjectId) : IRequest<List<IssuesDto>>;

public class GetIssuesQueryHandler : IRequestHandler<GetIssuesQuery, List<IssuesDto>>
{
    private readonly List<Project> _projects;
    private readonly IssuesDtoMapper _mapper;

    public GetIssuesQueryHandler(
        List<Project> projects,
        IssuesDtoMapper mapper)
    {
        _projects = projects;
        _mapper = mapper;
    }

    public async Task<List<IssuesDto>> Handle(GetIssuesQuery query, CancellationToken cancellationToken)
    {
        var project = _projects.FirstOrDefault(p => p.Id == query.ProjectId);

        if (project is null)
        {
            throw new ProjectNotFoundException(query.ProjectId);
        }

        return project.Issues.Select(_mapper.ToDto).ToList();
    }
}
