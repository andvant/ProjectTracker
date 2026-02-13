namespace ProjectTracker.Domain.Entities;

public class Attachment : AuditableEntity
{
    public string Name { get; private set; }
    public string StorageKey { get; private set; }

    public Attachment(string name, string storageKey)
    {
        Id = Guid.CreateVersion7();
        Name = name;
        StorageKey = storageKey;
        CreatedOn = DateTime.UtcNow; // TODO: move to DbContext
    }
}
