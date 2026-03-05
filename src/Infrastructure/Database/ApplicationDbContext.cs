using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Interfaces;
using ProjectTracker.Domain.Common;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Database;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly ICurrentUser _currentUser;
    private readonly TimeProvider _timeProvider;

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Issue> Issues => Set<Issue>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<Comment> Comments => Set<Comment>();

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        ICurrentUser currentUser,
        TimeProvider timeProvider)
        : base(options)
    {
        _currentUser = currentUser;
        _timeProvider = timeProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("project_tracker");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        UpdateAuditableEntities();

        return await base.SaveChangesAsync(ct);
    }

    public void Update(AuditableEntity entity)
    {
        base.Update(entity);
    }

    private void UpdateAuditableEntities()
    {
        var entries = ChangeTracker.Entries().Where(e =>
            e.Entity is AuditableEntity &&
            (e.State is EntityState.Added or EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (AuditableEntity)entry.Entity;

            var currentUserId = _currentUser.GetUserId();
            var currentTime = _timeProvider.GetUtcNow();

            if (entry.State == EntityState.Added)
            {
                entity.CreatedBy = currentUserId;
                entity.CreatedAt = currentTime;
            }

            entity.UpdatedBy = currentUserId;
            entity.UpdatedAt = currentTime;
        }
    }

    public async Task<IReadOnlyCollection<Guid>> GetProjectMemberIds(Guid projectId, CancellationToken ct)
    {
        return await Projects
            .Where(p => p.Id == projectId)
            .SelectMany(p => p.Members)
            .Select(m => m.UserId).ToListAsync(ct);
    }
}
