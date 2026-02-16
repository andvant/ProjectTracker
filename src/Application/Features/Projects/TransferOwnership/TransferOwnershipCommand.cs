using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Projects.TransferOwnership;

public record TransferOwnershipCommand(Guid ProjectId, Guid NewOwnerId) : IRequest;

internal class TransferOwnershipCommandHandler : IRequestHandler<TransferOwnershipCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<TransferOwnershipCommandHandler> _logger;

    public TransferOwnershipCommandHandler(
        IApplicationDbContext context,
        ILogger<TransferOwnershipCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task Handle(TransferOwnershipCommand command, CancellationToken ct)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == command.ProjectId, ct)
            ?? throw new ProjectNotFoundException(command.ProjectId);

        var newOwner = await _context.Users.FirstOrDefaultAsync(u => u.Id == command.NewOwnerId, ct)
            ?? throw new NewOwnerNotFoundException(command.NewOwnerId);

        project.TransferOwnership(newOwner);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Transferred ownership of project '{ProjectId}' to new Owner '{NewOwnerId}'",
            project.Id, newOwner.Id);
    }
}
