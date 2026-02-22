namespace ProjectTracker.Infrastructure.Identity;

public class KeycloakAdminConfig
{
    public required string ClientId { get; set; }
    public required string ClientSecret { get; set; }
    public required string Realm { get; set; }
}
