using Microsoft.EntityFrameworkCore;

namespace ProjectTracker.Application.Features.Issues.GetIssues;

public record GetIssuesQuery(Guid ProjectId) : IRequest<List<IssuesDto>>;

internal class GetIssuesQueryHandler : IRequestHandler<GetIssuesQuery, List<IssuesDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IssuesDtoMapper _mapper;

    public GetIssuesQueryHandler(
        IApplicationDbContext context,
        IssuesDtoMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<List<IssuesDto>> Handle(GetIssuesQuery query, CancellationToken ct)
    {
        var project = _context.Projects
            .Include(p => p.Issues)
            .FirstOrDefault(p => p.Id == query.ProjectId);

        if (project is null)
        {
            throw new ProjectNotFoundException(query.ProjectId);
        }

        return project.Issues.Select(_mapper.ToDto).ToList();
    }
}
