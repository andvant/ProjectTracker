using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Projects.UploadProjectAttachment;

public record UploadProjectAttachmentCommand(Guid ProjectId, string Name, Stream Stream, string MimeType) : IRequest;

internal class UploadProjectAttachmentCommandHandler : IRequestHandler<UploadProjectAttachmentCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IObjectStorage _storage;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<UploadProjectAttachmentCommandHandler> _logger;

    public UploadProjectAttachmentCommandHandler(
        IApplicationDbContext context,
        IObjectStorage storage,
        ICurrentUser currentUser,
        ILogger<UploadProjectAttachmentCommandHandler> logger)
    {
        _context = context;
        _storage = storage;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task Handle(UploadProjectAttachmentCommand command, CancellationToken ct)
    {
        var project = await _context.Projects.FirstOrDefaultAsync(p => p.Id == command.ProjectId, ct)
            ?? throw new ProjectNotFoundException(command.ProjectId);

        _currentUser.ValidateAllowed([project.OwnerId]);

        var storageKey = await _storage.UploadProjectAttachment(command.ProjectId,
            command.Name, command.Stream, command.MimeType, ct);

        project.AddAttachment(command.Name, storageKey, command.MimeType);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Uploaded attachment with storage key '{StorageKey}' for project with id '{Id}', key '{Key}'",
            storageKey, project.Id, project.Key);
    }
}

public class UploadProjectAttachmentCommandValidator : AbstractValidator<UploadProjectAttachmentCommand>
{
    public UploadProjectAttachmentCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().Must(n => !n.Contains('/'));
        RuleFor(c => c.MimeType).NotEmpty();
        RuleFor(c => c.Stream).NotNull();
    }
}
