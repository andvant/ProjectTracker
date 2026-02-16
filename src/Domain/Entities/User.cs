namespace ProjectTracker.Domain.Entities;

public class User : Entity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public DateTimeOffset RegistrationDate { get; private set; }

    public ICollection<Project> Projects { get; private set; } = new List<Project>();
    public ICollection<Issue> AssignedIssues { get; private set; } = new List<Issue>();
    public ICollection<Issue> WatchedIssues { get; private set; } = new List<Issue>();

    protected User()
    {
        Name = null!;
        Email = null!;
    }

    public User(string name, string email, DateTimeOffset registrationDate)
    {
        Name = name;
        Email = email;
        RegistrationDate = registrationDate;
    }
}
