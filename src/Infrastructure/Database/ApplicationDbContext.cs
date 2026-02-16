using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ProjectTracker.Application.Common;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Database;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Issue> Issues => Set<Issue>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Attachment> Attachments => Set<Attachment>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.EnableSensitiveDataLogging(); // TODO: remove in production env
    }
}
