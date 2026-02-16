using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Projects.RemoveMember;

public record RemoveMemberCommand(Guid ProjectId, Guid MemberId) : IRequest;

internal class RemoveMemberCommandHandler : IRequestHandler<RemoveMemberCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<RemoveMemberCommandHandler> _logger;

    public RemoveMemberCommandHandler(
        IApplicationDbContext context,
        ILogger<RemoveMemberCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(RemoveMemberCommand command, CancellationToken ct)
    {
        var project = _context.Projects.FirstOrDefault(p => p.Id == command.ProjectId)
            ?? throw new ProjectNotFoundException(command.ProjectId);

        var member = _context.Users.FirstOrDefault(u => u.Id == command.MemberId)
            ?? throw new MemberNotFoundException(command.MemberId);

        project.RemoveMember(member);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Removed member '{MemberId}' from project '{ProjectId}'",
            member.Id, project.Id);
    }
}
