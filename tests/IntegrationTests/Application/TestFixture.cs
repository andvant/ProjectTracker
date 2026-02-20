using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ProjectTracker.Application;
using ProjectTracker.Application.Interfaces;
using ProjectTracker.Infrastructure.Database;

namespace ProjectTracker.IntegrationTests.Application;

public class TestFixture : IAsyncDisposable
{
    private readonly ServiceProvider _provider;
    private readonly SqliteConnection _connection;

    public IServiceScope CreateScope() => _provider.CreateScope();

    public TestFixture()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var services = new ServiceCollection();

        services.AddApplicationServices();
        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(_connection));
        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());
        services.AddSingleton<TimeProvider, FakeTimeProvider>();
        services.AddScoped<ICurrentUser, FakeCurrentUser>();
        services.AddScoped(typeof(ILogger<>), typeof(NullLogger<>));

        _provider = services.BuildServiceProvider();

        using var scope = _provider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.Database.EnsureCreated();
    }

    public async ValueTask DisposeAsync()
    {
        await _provider.DisposeAsync();
        await _connection.DisposeAsync();
    }
}

public class FakeTimeProvider : TimeProvider
{
    public override DateTimeOffset GetUtcNow() => FakeTime;
    public DateTimeOffset FakeTime { get; set; }
}

public class FakeCurrentUser : ICurrentUser
{
    public Guid GetUserId() => FakeUserId;
    public Guid FakeUserId { get; set; }
}
