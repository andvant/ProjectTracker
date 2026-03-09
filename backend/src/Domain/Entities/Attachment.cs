namespace ProjectTracker.Domain.Entities;

public class Attachment : AuditableEntity
{
    public string Name { get; private set; }
    public string StorageKey { get; private set; }
    public string MimeType { get; private set; }

    // for EF Core
    protected Attachment()
    {
        Name = null!;
        StorageKey = null!;
        MimeType = null!;
    }

    public Attachment(string name, string storageKey, string mimeType)
    {
        Name = name;
        StorageKey = storageKey;
        MimeType = mimeType;
    }
}
