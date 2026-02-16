using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectTracker.Domain.Entities;

namespace ProjectTracker.Infrastructure.Database.Configurations;

internal class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever();

        builder.HasIndex(e => e.Key).IsUnique();

        builder.Property(e => e.Key).HasConversion(Converters.ProjectKeyConverter);
        builder.Property(e => e.Name).HasConversion(Converters.TitleConverter);

        builder.HasOne(p => p.Owner).WithMany().HasForeignKey(p => p.OwnerId);
        builder.HasMany(p => p.Issues).WithOne(i => i.Project).HasForeignKey(i => i.ProjectId);
        builder.HasMany(p => p.Members).WithOne(m => m.Project).HasForeignKey(m => m.ProjectId);
        builder.HasMany(p => p.Attachments).WithOne();
    }
}
