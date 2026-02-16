using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Database.Configurations;

internal class IssueConfiguration : IEntityTypeConfiguration<Issue>
{
    public void Configure(EntityTypeBuilder<Issue> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.HasIndex(e => e.Key).IsUnique();

        builder.Property(e => e.Key).HasConversion(Converters.IssueKeyConverter);
        builder.Property(e => e.Title).HasConversion(Converters.TitleConverter);

        builder.HasOne(i => i.Assignee).WithMany(i => i.AssignedIssues).HasForeignKey(i => i.AssigneeId);
        builder.HasOne(i => i.Reporter).WithMany().HasForeignKey(i => i.ReporterId);
        builder.HasOne(i => i.ParentIssue).WithMany(p => p.ChildIssues).HasForeignKey(i => i.ParentIssueId);
        builder.HasMany(i => i.Watchers).WithOne(w => w.Issue);
        builder.HasMany(i => i.Attachments).WithOne();
    }
}
