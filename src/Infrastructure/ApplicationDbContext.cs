using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ProjectTracker.Application.Common;
using ProjectTracker.Domain.Entities;
using ProjectTracker.Domain.ValueObjects;

namespace ProjectTracker.Infrastructure;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Issue> Issues => Set<Issue>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Attachment> Attachments => Set<Attachment>();

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

internal class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.HasIndex(e => e.Key).IsUnique();

        var keyConverter = new ValueConverter<ProjectKey, string>(v => v, v => v);
        builder.Property(e => e.Key).HasConversion(keyConverter);

        var titleConverter = new ValueConverter<Title, string>(v => v, v => v);
        builder.Property(e => e.Name).HasConversion(titleConverter);

        builder.HasOne(p => p.Owner).WithMany().HasForeignKey(p => p.OwnerId);
        builder.HasMany(p => p.Issues).WithOne(i => i.Project).HasForeignKey(i => i.ProjectId);
        builder.HasMany(p => p.Members).WithMany();
        builder.HasMany(p => p.Attachments).WithOne();
    }
}

internal class IssueConfiguration : IEntityTypeConfiguration<Issue>
{
    public void Configure(EntityTypeBuilder<Issue> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.HasIndex(e => e.Key).IsUnique();

        var keyConverter = new ValueConverter<IssueKey, string>(v => v, v => IssueKey.Parse(v));
        builder.Property(e => e.Key).HasConversion(keyConverter);

        var titleConverter = new ValueConverter<Title, string>(v => v, v => v);
        builder.Property(e => e.Title).HasConversion(titleConverter);

        builder.HasOne(i => i.Assignee).WithMany().HasForeignKey(i => i.AssigneeId);
        builder.HasOne(i => i.Project).WithMany(p => p.Issues).HasForeignKey(i => i.ProjectId);
        builder.HasOne(i => i.Reporter).WithMany().HasForeignKey(i => i.ReporterId);
        builder.HasOne(i => i.ParentIssue).WithMany(p => p.ChildIssues).HasForeignKey(i => i.ParentIssueId);
        builder.HasMany(i => i.Watchers).WithMany();
        builder.HasMany(p => p.Attachments).WithOne();
    }
}

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
    }
}

internal class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();
    }
}
