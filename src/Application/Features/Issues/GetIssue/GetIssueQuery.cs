namespace ProjectTracker.Application.Features.Issues.GetIssue;

public record GetIssueQuery(Guid ProjectId, Guid IssueId) : IRequest<IssueDto>;

internal class GetIssueQueryHandler : IRequestHandler<GetIssueQuery, IssueDto>
{
    private readonly List<Project> _projects;
    private readonly IssueDtoMapper _mapper;

    public GetIssueQueryHandler(
        List<Project> projects,
        IssueDtoMapper mapper)
    {
        _projects = projects;
        _mapper = mapper;
    }

    public async Task<IssueDto> Handle(GetIssueQuery query, CancellationToken ct)
    {
        var project = _projects.FirstOrDefault(p => p.Id == query.ProjectId);

        if (project is null)
        {
            throw new ProjectNotFoundException(query.ProjectId);
        }

        var issue = project.Issues.FirstOrDefault(i => i.Id == query.IssueId)
            ?? throw new IssueNotFoundException(query.IssueId);

        return _mapper.ToDto(issue);
    }
}
