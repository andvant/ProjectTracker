namespace ProjectTracker.Application.Features.Projects.GetProjects;

public record GetProjectsQuery : IRequest<List<ProjectsDto>>;

internal class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, List<ProjectsDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetProjectsQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<List<ProjectsDto>> Handle(GetProjectsQuery query, CancellationToken ct)
    {
        if (_currentUser.IsAdmin() || _currentUser.IsProjectManager())
        {
            return await _context.Projects.AsNoTracking().ProjectToDto().ToListAsync(ct);
        }

        var currentUserId = _currentUser.GetUserId();

        var availableProjectIds = await _context.Projects
            .SelectMany(p => p.Members)
            .Where(m => m.UserId == currentUserId)
            .Select(m => m.ProjectId)
            .ToListAsync(ct);

        return await _context.Projects.AsNoTracking()
            .Where(p => availableProjectIds.Contains(p.Id))
            .ProjectToDto()
            .ToListAsync(ct);
    }
}
