namespace ProjectTracker.Domain.Entities;

public class Attachment : AuditableEntity
{
    public string Name { get; private set; }
    public string StorageKey { get; private set; }

    protected Attachment()
    {
        Name = null!;
        StorageKey = null!;
    }

    public Attachment(string name, string storageKey)
    {
        Name = name;
        StorageKey = storageKey;
    }
}
