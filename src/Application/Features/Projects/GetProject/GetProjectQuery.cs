using ProjectTracker.Application.Common;

namespace ProjectTracker.Application.Features.Projects.GetProject;

public record GetProjectQuery(Guid Id) : IRequest<ProjectDto>;

internal class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, ProjectDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetProjectQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<ProjectDto> Handle(GetProjectQuery query, CancellationToken ct)
    {
        var memberIds = await _context.Projects
            .Where(p => p.Id == query.Id)
            .SelectMany(p => p.Members)
            .Select(m => m.UserId).ToListAsync(ct);

        _currentUser.ValidateAllowed(memberIds, [Roles.ProjectManager]);

        return await _context.Projects.AsNoTracking()
            .Where(p => p.Id == query.Id)
            .ProjectToDto()
            .FirstOrDefaultAsync(ct) ?? throw new ProjectNotFoundException(query.Id);
    }
}
