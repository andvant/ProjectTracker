using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Projects.AddMember;

public record AddMemberCommand(Guid ProjectId, Guid MemberId) : IRequest;

internal class AddMemberCommandHandler : IRequestHandler<AddMemberCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<AddMemberCommandHandler> _logger;

    public AddMemberCommandHandler(
        IApplicationDbContext context,
        ILogger<AddMemberCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(AddMemberCommand command, CancellationToken ct)
    {
        var project = _context.Projects.FirstOrDefault(p => p.Id == command.ProjectId)
            ?? throw new ProjectNotFoundException(command.ProjectId);

        var member = _context.Users.FirstOrDefault(u => u.Id == command.MemberId)
            ?? throw new MemberNotFoundException(command.MemberId);

        project.AddMember(member);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Added member '{MemberId}' to project '{ProjectId}'",
            member.Id, project.Id);
    }
}
