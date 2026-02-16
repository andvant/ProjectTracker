using Microsoft.EntityFrameworkCore;

namespace ProjectTracker.Application.Features.Issues.GetIssues;

public record GetIssuesQuery(Guid ProjectId) : IRequest<List<IssuesDto>>;

internal class GetIssuesQueryHandler : IRequestHandler<GetIssuesQuery, List<IssuesDto>>
{
    private readonly IApplicationDbContext _context;

    public GetIssuesQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<IssuesDto>> Handle(GetIssuesQuery query, CancellationToken ct)
    {
        var projectExists = await _context.Projects.AnyAsync(p => p.Id == query.ProjectId);

        if (!projectExists)
        {
            throw new ProjectNotFoundException(query.ProjectId);
        }

        return await _context.Projects
            .Where(p => p.Id == query.ProjectId)
            .SelectMany(p => p.Issues)
            .ProjectToDto()
            .ToListAsync(ct);
    }
}
