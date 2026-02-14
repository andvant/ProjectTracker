using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Projects.AddMember;

public record AddMemberCommand(Guid ProjectId, Guid MemberId) : IRequest;

internal class AddMemberCommandHandler : IRequestHandler<AddMemberCommand>
{
    private readonly List<Project> _projects;
    private readonly List<User> _users;
    private readonly ILogger<AddMemberCommandHandler> _logger;

    public AddMemberCommandHandler(
        List<Project> projects,
        List<User> users,
        ILogger<AddMemberCommandHandler> logger)
    {
        _projects = projects;
        _users = users;
        _logger = logger;
    }

    public async Task Handle(AddMemberCommand command, CancellationToken ct)
    {
        var project = _projects.FirstOrDefault(p => p.Id == command.ProjectId)
            ?? throw new ProjectNotFoundException(command.ProjectId);

        var member = _users.FirstOrDefault(u => u.Id == command.MemberId)
            ?? throw new MemberNotFoundException(command.MemberId);

        project.AddMember(member);

        _logger.LogInformation(
            "Added member '{MemberId}' to project '{ProjectId}'",
            member.Id, project.Id);
    }
}
