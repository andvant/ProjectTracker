namespace ProjectTracker.Domain.Entities;

public class IssueAttachment
{
    public Guid IssueId { get; private set; }
    public Issue Issue { get; private set; }
    public Guid AttachmentId { get; private set; }
    public Attachment Attachment { get; private set; }

    // for EF Core
    protected IssueAttachment()
    {
        Issue = null!;
        Attachment = null!;
    }

    public IssueAttachment(Issue issue, Attachment attachment)
    {
        Issue = issue;
        IssueId = issue.Id;
        Attachment = attachment;
        AttachmentId = attachment.Id;
    }
}
