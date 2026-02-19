var builder = DistributedApplication.CreateBuilder(args);

var postgresPassword = builder.AddParameter("PostgresPassword", "postgres", secret: true);
var keycloakUsername = builder.AddParameter("KeycloakUsername", value: "kc_admin");
var keycloakPassword = builder.AddParameter("KeycloakPassword", secret: true, value: "kc_secret");
var keycloakPostgresUser = builder.AddParameter("KeycloakPostgresUser", value: "kc_db_user");
var keycloakPostgresPassword = builder.AddParameter("KeycloakPostgresPassword", secret: true, value: "kc_db_secret");

var postgres = builder.AddPostgres("postgres", password: postgresPassword)
    .WithImageTag("18")
    .WithEnvironment("POSTGRES_USER", "postgres")
    .WithHostPort(5432)
    .WithVolume("projecttracker_postgres_data", "/var/lib/postgresql")
    .WithInitFiles("../../postgres");

var projectTrackerDb = postgres.AddDatabase("ProjectTrackerDb", "project_tracker_db");
var keycloakDb = postgres.AddDatabase("KeycloakDb", "keycloak_db");

var rustfs = builder.AddContainer("rustfs", "rustfs/rustfs")
    .WithImageTag("1.0.0-alpha.83")
    .WithEnvironment("RUSTFS_CONSOLE_ENABLE", "true")
    .WithEnvironment("RUSTFS_ACCESS_KEY", "rf_admin")
    .WithEnvironment("RUSTFS_SECRET_KEY", "rf_secret")
    .WithHttpEndpoint(port: 9000, targetPort: 9000, name: "s3")
    .WithHttpEndpoint(port: 9001, targetPort: 9001, name: "console")
    .WithVolume("projecttracker_rustfs_data", "/data");

var keycloak = builder.AddKeycloak("keycloak", 8080, keycloakUsername, keycloakPassword)
    .WithImageTag("26.5.2")
    .WithEnvironment("KC_HEALTH_ENABLED", "true")
    .WithEnvironment("KC_DB", "postgres")
    .WithEnvironment("KC_DB_URL", $"jdbc:postgresql://{postgres.Resource.Name}:{postgres.Resource.Port}/{keycloakDb.Resource.DatabaseName}")
    .WithEnvironment("KC_DB_USERNAME", keycloakPostgresUser)
    .WithEnvironment("KC_DB_PASSWORD", keycloakPostgresPassword)
    .WithReference(postgres)
    .WaitFor(postgres);

var api = builder.AddProject<Projects.Api>("api")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Development")
    .WithReference(projectTrackerDb)
    .WaitFor(projectTrackerDb)
    .WaitFor(rustfs)
    .WaitFor(keycloak);

builder.Build().Run();
