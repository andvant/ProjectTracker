namespace ProjectTracker.Domain.Entities;

public class Attachment : AuditableEntity
{
    public string Name { get; }
    public string StorageKey { get; }

    public Attachment(string name, string storageKey)
    {
        Name = name;
        StorageKey = storageKey;
    }
}
