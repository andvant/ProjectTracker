using ProjectTracker.Application.Features.Issues.Common;

namespace ProjectTracker.Application.Features.Issues.GetIssue;

public record GetIssueQuery(Guid ProjectId, Guid IssueId) : IRequest<IssueDto?>;

public class GetIssueQueryHandler : IRequestHandler<GetIssueQuery, IssueDto?>
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

    public async Task<IssueDto?> Handle(GetIssueQuery query, CancellationToken cancellationToken)
    {
        var project = _projects.FirstOrDefault(p => p.Id == query.ProjectId);

        if (project is null)
        {
            throw new ApplicationException($"Project with id {query.ProjectId} not found");
        }

        var issue = project.Issues.FirstOrDefault(i => i.Id == query.IssueId);

        return issue is null ? null : _mapper.ToDto(issue);
    }
}
