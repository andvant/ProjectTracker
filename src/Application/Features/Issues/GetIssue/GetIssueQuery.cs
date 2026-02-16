namespace ProjectTracker.Application.Features.Issues.GetIssue;

public record GetIssueQuery(Guid ProjectId, Guid IssueId) : IRequest<IssueDto>;

internal class GetIssueQueryHandler : IRequestHandler<GetIssueQuery, IssueDto>
{
    private readonly IApplicationDbContext _context;

    public GetIssueQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IssueDto> Handle(GetIssueQuery query, CancellationToken ct)
    {
        var project = _context.Projects.FirstOrDefault(p => p.Id == query.ProjectId);

        if (project is null)
        {
            throw new ProjectNotFoundException(query.ProjectId);
        }

        var issue = project.Issues.FirstOrDefault(i => i.Id == query.IssueId)
            ?? throw new IssueNotFoundException(query.IssueId);

        return issue.ToDto();
    }
}
