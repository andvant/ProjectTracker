namespace ProjectTracker.Domain.Entities;

public class User
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }

    [SetsRequiredMembers]
    public User(string name, string email)
    {
        Id = Guid.CreateVersion7();
        Name = name;
        Email = email;
    }
}
