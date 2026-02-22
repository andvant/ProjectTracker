namespace ProjectTracker.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Project> Projects { get; }
    DbSet<User> Users { get; }
    Task<IReadOnlyCollection<Guid>> GetProjectMemberIds(Guid projectId, CancellationToken ct);

    Task<int> SaveChangesAsync(CancellationToken ct);
}
