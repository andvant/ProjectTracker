namespace ProjectTracker.Domain.Entities;

public class ProjectMember
{
    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; }
    public DateTimeOffset MemberSince { get; private set; }

    // For EF Core
    protected ProjectMember()
    {
        Project = null!;
        User = null!;
    }

    public ProjectMember(Project project, User member, DateTimeOffset memberSince)
    {
        Project = project;
        User = member;
        MemberSince = memberSince;
    }
}
