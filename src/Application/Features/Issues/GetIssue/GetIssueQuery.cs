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
        var projectExists = await _context.Projects.AnyAsync(p => p.Id == query.ProjectId, ct);

        if (!projectExists)
        {
            throw new ProjectNotFoundException(query.ProjectId);
        }

        return await _context.Projects.AsNoTracking()
            .Where(p => p.Id == query.ProjectId)
            .SelectMany(p => p.Issues)
            .Where(i => i.Id == query.IssueId)
            .ProjectToDto()
            .FirstOrDefaultAsync(ct)
            ?? throw new IssueNotFoundException(query.IssueId);
    }
}
