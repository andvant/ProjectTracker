using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Projects.AddMember;

public record AddMemberCommand(Guid ProjectId, Guid MemberId) : IRequest;

internal class AddMemberCommandHandler : IRequestHandler<AddMemberCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly TimeProvider _timeProvider;
    private readonly ILogger<AddMemberCommandHandler> _logger;

    public AddMemberCommandHandler(
        IApplicationDbContext context,
        ICurrentUser currentUser,
        TimeProvider timeProvider,
        ILogger<AddMemberCommandHandler> logger)
    {
        _context = context;
        _currentUser = currentUser;
        _timeProvider = timeProvider;
        _logger = logger;
    }

    public async Task Handle(AddMemberCommand command, CancellationToken ct)
    {
        var project = await _context.Projects
            .Include(p => p.Members)
            .FirstOrDefaultAsync(p => p.Id == command.ProjectId, ct)
            ?? throw new ProjectNotFoundException(command.ProjectId);

        _currentUser.ValidateAllowed([project.OwnerId]);

        var member = await _context.Users.FirstOrDefaultAsync(u => u.Id == command.MemberId, ct)
            ?? throw new MemberNotFoundException(command.MemberId);

        project.AddMember(member, _timeProvider.GetUtcNow());

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Added member '{MemberId}' to project '{ProjectId}'",
            member.Id, project.Id);
    }
}
