using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Projects.RemoveMember;

public record RemoveMemberCommand(Guid ProjectId, Guid MemberId) : IRequest;

internal class RemoveMemberCommandHandler : IRequestHandler<RemoveMemberCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<RemoveMemberCommandHandler> _logger;

    public RemoveMemberCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser,
        ILogger<RemoveMemberCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task Handle(RemoveMemberCommand command, CancellationToken ct)
    {
        var project = await _context.Projects
            .Include(p => p.Members)
            .Include(p => p.Issues).ThenInclude(i => i.Watchers)
            .FirstOrDefaultAsync(p => p.Id == command.ProjectId, ct)
            ?? throw new ProjectNotFoundException(command.ProjectId);

        _currentUser.ValidateAllowed([project.OwnerId]);

        var member = await _context.Users.FirstOrDefaultAsync(u => u.Id == command.MemberId, ct)
            ?? throw new MemberNotFoundException(command.MemberId);

        project.RemoveMember(member);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Removed member '{MemberId}' from project '{ProjectId}'",
            member.Id, project.Id);
    }
}
