namespace ProjectTracker.Domain.Entities;

public class ProjectAttachment
{
    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; }
    public Guid AttachmentId { get; private set; }
    public Attachment Attachment { get; private set; }

    // for EF Core
    protected ProjectAttachment()
    {
        Project = null!;
        Attachment = null!;
    }

    public ProjectAttachment(Project project, Attachment attachment)
    {
        Project = project;
        ProjectId = project.Id;
        Attachment = attachment;
        AttachmentId = attachment.Id;
    }
}
