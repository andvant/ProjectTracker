using Microsoft.Extensions.Logging;

namespace ProjectTracker.Application.Features.Issues.UploadIssueAttachment;

public record UploadIssueAttachmentCommand(
    Guid ProjectId,
    Guid IssueId,
    string Name,
    Stream Stream,
    string MimeType) : IRequest;

internal class UploadIssueAttachmentCommandHandler : IRequestHandler<UploadIssueAttachmentCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IObjectStorage _storage;
    private readonly ICurrentUser _currentUser;
    private readonly ILogger<UploadIssueAttachmentCommandHandler> _logger;

    public UploadIssueAttachmentCommandHandler(
        IApplicationDbContext context,
        IObjectStorage storage,
        ICurrentUser currentUser,
        ILogger<UploadIssueAttachmentCommandHandler> logger)
    {
        _context = context;
        _storage = storage;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task Handle(UploadIssueAttachmentCommand command, CancellationToken ct)
    {
        var projectExists = await _context.Projects.AnyAsync(p => p.Id == command.ProjectId, ct);

        if (!projectExists)
        {
            throw new ProjectNotFoundException(command.ProjectId);
        }

        var issue = await _context.Projects
            .Where(p => p.Id == command.ProjectId)
            .SelectMany(p => p.Issues)
            .FirstOrDefaultAsync(i => i.Id == command.IssueId, ct)
            ?? throw new IssueNotFoundException(command.IssueId);

        _currentUser.ValidateAllowed([issue.ReporterId, issue.Project.OwnerId]);

        var storageKey = await _storage.UploadIssueAttachment(command.ProjectId, issue.Id,
            command.Name, command.Stream, command.MimeType, ct);

        issue.AddAttachment(command.Name, storageKey, command.MimeType);

        await _context.SaveChangesAsync(ct);

        _logger.LogInformation(
            "Uploaded attachment with storage key '{StorageKey}' for issue with id '{Id}', key '{Key}'",
            storageKey, issue.Id, issue.Key);
    }
}

public class UploadIssueAttachmentCommandValidator : AbstractValidator<UploadIssueAttachmentCommand>
{
    public UploadIssueAttachmentCommandValidator()
    {
        RuleFor(c => c.Name).NotEmpty().Must(n => !n.Contains('/'));
        RuleFor(c => c.MimeType).NotEmpty();
        RuleFor(c => c.Stream).NotNull();
    }
}
