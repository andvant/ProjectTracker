namespace ProjectTracker.Application.Exceptions;

public class AttachmentNotFoundException : ApplicationException
{
    public Guid AttachmentId { get; }

    public AttachmentNotFoundException(Guid attachmentId)
        : base($"Attachment with id '{attachmentId}' was not found.")
    {
        AttachmentId = attachmentId;
    }
}
