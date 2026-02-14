using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Projects.TransferOwnership;

public record TransferOwnershipCommand(Guid ProjectId, Guid NewOwnerId) : IRequest;

internal class TransferOwnershipCommandHandler : IRequestHandler<TransferOwnershipCommand>
{
    private readonly List<Project> _projects;
    private readonly List<User> _users;
    private readonly ILogger<TransferOwnershipCommandHandler> _logger;

    public TransferOwnershipCommandHandler(
        List<Project> projects,
        List<User> users,
        ILogger<TransferOwnershipCommandHandler> logger)
    {
        _projects = projects;
        _users = users;
        _logger = logger;
    }

    public async Task Handle(TransferOwnershipCommand command, CancellationToken ct)
    {
        var project = _projects.FirstOrDefault(p => p.Id == command.ProjectId)
            ?? throw new ProjectNotFoundException(command.ProjectId);

        var newOwner = _users.FirstOrDefault(u => u.Id == command.NewOwnerId)
            ?? throw new NewOwnerNotFoundException(command.NewOwnerId);

        project.TransferOwnership(newOwner);

        _logger.LogInformation(
            "Transferred ownership of project '{ProjectId}' to new Owner '{NewOwnerId}'",
            project.Id, newOwner.Id);
    }
}
