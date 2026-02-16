using Microsoft.EntityFrameworkCore;

namespace ProjectTracker.Application.Features.Projects.GetProject;

public record GetProjectQuery(Guid Id) : IRequest<ProjectDto>;

internal class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, ProjectDto>
{
    private readonly IApplicationDbContext _context;

    public GetProjectQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProjectDto> Handle(GetProjectQuery query, CancellationToken ct) =>
        await _context.Projects
            .Where(p => p.Id == query.Id)
            .ProjectToDto()
            .FirstOrDefaultAsync(ct) ?? throw new ProjectNotFoundException(query.Id);
}
