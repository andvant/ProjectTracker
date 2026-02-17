using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProjectTracker.Application.Common;
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
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        UpdateAuditableEntities();

        return await base.SaveChangesAsync(ct);
    }

    private void UpdateAuditableEntities()
    {
        var entries = ChangeTracker.Entries().Where(e =>
            e.Entity is AuditableEntity &&
            (e.State is EntityState.Added or EntityState.Modified));

        foreach (var entry in entries)
        {
            var entity = (AuditableEntity)entry.Entity;

            if (entry.State == EntityState.Added)
            {
                entity.CreatedBy = _currentUser.UserId;
                entity.CreatedOn = _timeProvider.GetUtcNow();
            }

            entity.UpdatedBy = _currentUser.UserId;
            entity.UpdatedOn = _timeProvider.GetUtcNow();
        }
    }
}
