using Microsoft.EntityFrameworkCore;

namespace ProjectTracker.Application.Common;

public interface IApplicationDbContext
{
    DbSet<Project> Projects { get; }
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken ct);
}
