using System.Reflection;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Interfaces;
using ProjectTracker.Domain.Common;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Database;

internal class ApplicationDbContext : DbContext, IApplicationDbContext
{
    private readonly IPublisher _publisher;
    private readonly ICurrentUser _currentUser;
    private readonly TimeProvider _timeProvider;

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Issue> Issues => Set<Issue>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Notification> Notifications => Set<Notification>();

    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options,
        IPublisher publisher,
        ICurrentUser currentUser,
        TimeProvider timeProvider)
        : base(options)
    {
        _publisher = publisher;
        _currentUser = currentUser;
        _timeProvider = timeProvider;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("project_tracker");
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        UpdateAuditableEntities();

        var result = await base.SaveChangesAsync(ct);

        await PublishDomainEvents(ct);

        return result;
    }

    public void Update(AuditableEntity entity)
    {
        base.Update(entity);
    }

    public async Task<IReadOnlyCollection<Guid>> GetProjectMemberIds(Guid projectId, CancellationToken ct)
    {
        return await Projects
            .Where(p => p.Id == projectId)
            .SelectMany(p => p.Members)
            .Select(m => m.UserId).ToListAsync(ct);
    }

    private void UpdateAuditableEntities()
    {
        var entries = ChangeTracker
            .Entries<AuditableEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified)
            .ToList();

        foreach (var entry in entries)
        {
            var entity = entry.Entity;

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

    private async Task PublishDomainEvents(CancellationToken ct)
    {
        var entities = ChangeTracker
            .Entries<Entity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .ToList();

        var domainEvents = entities.SelectMany(e => e.DomainEvents).ToList();

        foreach (var entity in entities)
        {
            entity.ClearDomainEvents();
        }

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent, ct);
        }
    }
}
