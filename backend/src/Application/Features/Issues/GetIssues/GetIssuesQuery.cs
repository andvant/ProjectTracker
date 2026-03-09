using ProjectTracker.Application.Common;

namespace ProjectTracker.Application.Features.Issues.GetIssues;

public record GetIssuesQuery(Guid ProjectId) : IRequest<IReadOnlyCollection<IssuesDto>>;

internal class GetIssuesQueryHandler : IRequestHandler<GetIssuesQuery, IReadOnlyCollection<IssuesDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetIssuesQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyCollection<IssuesDto>> Handle(GetIssuesQuery query, CancellationToken ct)
    {
        var projectExists = await _context.Projects.AnyAsync(p => p.Id == query.ProjectId, ct);

        if (!projectExists)
        {
            throw new ProjectNotFoundException(query.ProjectId);
        }

        var memberIds = await _context.GetProjectMemberIds(query.ProjectId, ct);

        _currentUser.ValidateAllowed(memberIds, [Roles.ProjectManager]);

        return await _context.Projects.AsNoTracking()
            .Where(p => p.Id == query.ProjectId)
            .SelectMany(p => p.Issues)
            .ProjectToDto()
            .ToListAsync(ct);
    }
}
