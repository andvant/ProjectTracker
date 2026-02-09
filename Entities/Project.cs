namespace ProjectTracker.Entities;

public class Project
{
    public required Guid Id { get; set; }
    public required string ShortName { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public List<Attachment> Attachments { get; set; } = new();
    public required User Owner { get; set; }
    public List<User> Members { get; set; } = new();
    public List<Issue> Issues { get; set; } = new();

    public Project(string shortName, string name, User owner)
    {
        ShortName = shortName;
        Name = name;
        Owner = owner;
        Members.Add(owner);
    }
}
