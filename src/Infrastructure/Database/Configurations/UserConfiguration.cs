using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Database.Configurations;

internal class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.HasMany(u => u.Projects).WithOne(p => p.User).HasForeignKey(p => p.UserId);
        builder.HasMany(u => u.WatchedIssues).WithOne(i => i.User).HasForeignKey(i => i.UserId);
    }
}
