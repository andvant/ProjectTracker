namespace ProjectTracker.Domain.Entities;

public class User : Entity
{
    public string Name { get; private set; }
    public string Email { get; private set; }
    public DateTimeOffset RegistrationDate { get; private set; }

    public IReadOnlyCollection<ProjectMember> Projects { get; private set; }
    public IReadOnlyCollection<Issue> AssignedIssues { get; private set; }
    public IReadOnlyCollection<IssueWatcher> WatchedIssues { get; private set; }

    // For EF Core
    protected User()
    {
        Name = null!;
        Email = null!;
        Projects = null!;
        AssignedIssues = null!;
        WatchedIssues = null!;
    }

    public User(string name, string email, DateTimeOffset registrationDate)
    {
        Name = name;
        Email = email;
        RegistrationDate = registrationDate;

        Projects = new List<ProjectMember>();
        AssignedIssues = new List<Issue>();
        WatchedIssues = new List<IssueWatcher>();
    }
}
