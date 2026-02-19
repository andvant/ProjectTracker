var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.Api>("api")
    .WithHttpEndpoint(5050);

builder.Build().Run();
