namespace ProjectTracker.Domain.Entities;

public class User : Entity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public DateTimeOffset RegistrationDate { get; private set; }

    public ICollection<ProjectMember> Projects { get; private set; } = new List<ProjectMember>();
    public ICollection<Issue> AssignedIssues { get; private set; } = new List<Issue>();
    public ICollection<IssueWatcher> WatchedIssues { get; private set; } = new List<IssueWatcher>();

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
