var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgres("postgres")
    .WithImageTag("18")
    .WithVolume("projecttracker_postgres_data", "/var/lib/postgresql")
    .WithHostPort(5432)
    .AddDatabase("ProjectTrackerDb");

builder.AddProject<Projects.Api>("api")
    .WithReference(database)
    .WaitFor(database);

builder.Build().Run();
