var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("PostgresPassword", "postgres", secret: true);

var database = builder.AddPostgres("postgres", password: postgresPassword)
    .WithImageTag("18")
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithEnvironment("POSTGRES_PASSWORD", "postgres")
    .WithHostPort(5432)
    .WithVolume("projecttracker_postgres_data", "/var/lib/postgresql")
    .AddDatabase("ProjectTrackerDb", "project_tracker_db");

var rustfs = builder.AddContainer("rustfs", "rustfs/rustfs")
    .WithImageTag("1.0.0-alpha.83")
    .WithEnvironment("RUSTFS_CONSOLE_ENABLE", "true")
    .WithEnvironment("RUSTFS_ACCESS_KEY", "admin")
    .WithEnvironment("RUSTFS_SECRET_KEY", "secret")
    .WithHttpEndpoint(port: 9000, targetPort: 9000, name: "s3")
    .WithHttpEndpoint(port: 9001, targetPort: 9001, name: "console")
    .WithVolume("projecttracker_rustfs_data", "/data");

var api = builder.AddProject<Projects.Api>("api")
    .WithReference(database)
    .WaitFor(database)
    .WaitFor(rustfs);

builder.Build().Run();
