using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Database.Configurations;

internal class IssueWatcherConfiguration : IEntityTypeConfiguration<IssueWatcher>
{
    public void Configure(EntityTypeBuilder<IssueWatcher> builder)
    {
        builder.HasKey(e => new { e.IssueId, e.UserId });
    }
}
