using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Projects.RemoveMember;

public record RemoveMemberCommand(Guid ProjectId, Guid MemberId) : IRequest;

internal class RemoveMemberCommandHandler : IRequestHandler<RemoveMemberCommand>
{
    private readonly List<Project> _projects;
    private readonly List<User> _users;
    private readonly ILogger<RemoveMemberCommandHandler> _logger;

    public RemoveMemberCommandHandler(
        List<Project> projects,
        List<User> users,
        ILogger<RemoveMemberCommandHandler> logger)
    {
        _projects = projects;
        _users = users;
        _logger = logger;
    }

    public async Task Handle(RemoveMemberCommand command, CancellationToken ct)
    {
        var project = _projects.FirstOrDefault(p => p.Id == command.ProjectId)
            ?? throw new ProjectNotFoundException(command.ProjectId);

        var member = _users.FirstOrDefault(u => u.Id == command.MemberId)
            ?? throw new MemberNotFoundException(command.MemberId);

        project.RemoveMember(member);

        _logger.LogInformation(
            "Removed member '{MemberId}' from project '{ProjectId}'",
            member.Id, project.Id);
    }
}
