namespace ProjectTracker.Entities;

public class Attachment
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string StorageKey { get; set; }
}
