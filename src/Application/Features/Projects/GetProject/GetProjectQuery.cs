using Microsoft.EntityFrameworkCore;

namespace ProjectTracker.Application.Features.Projects.GetProject;

public record GetProjectQuery(Guid Id) : IRequest<ProjectDto>;

internal class GetProjectQueryHandler : IRequestHandler<GetProjectQuery, ProjectDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ProjectDtoMapper _mapper;

    public GetProjectQueryHandler(
        IApplicationDbContext context,
        ProjectDtoMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ProjectDto> Handle(GetProjectQuery query, CancellationToken ct)
    {
        var project = _context.Projects.FirstOrDefault(p => p.Id == query.Id)
            ?? throw new ProjectNotFoundException(query.Id);

        return _mapper.ToDto(project);
    }
}
