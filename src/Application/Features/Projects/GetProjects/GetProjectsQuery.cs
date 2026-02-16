namespace ProjectTracker.Application.Features.Projects.GetProjects;

public record GetProjectsQuery : IRequest<List<ProjectsDto>>;

internal class GetProjectsQueryHandler : IRequestHandler<GetProjectsQuery, List<ProjectsDto>>
{
    private readonly IApplicationDbContext _context;

    public GetProjectsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public Task<List<ProjectsDto>> Handle(GetProjectsQuery query, CancellationToken ct) =>
        _context.Projects.ProjectToDto().ToListAsync(ct);
}
