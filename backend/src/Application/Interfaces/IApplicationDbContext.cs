using ProjectTracker.Domain.Common;

namespace ProjectTracker.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Project> Projects { get; }
    DbSet<Issue> Issues { get; }
    DbSet<User> Users { get; }
    DbSet<Attachment> Attachments { get; }
    DbSet<Notification> Notifications { get; }

    Task<IReadOnlyCollection<Guid>> GetProjectMemberIds(Guid projectId, CancellationToken ct);
    void Update(AuditableEntity entity); // to force setting UpdatedAt and UpdatedBy

    Task<int> SaveChangesAsync(CancellationToken ct);
}
