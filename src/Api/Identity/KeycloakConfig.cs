namespace ProjectTracker.Api.Identity;

public class KeycloakConfig
{
    public required bool RequireHttps { get; set; }
    public required string Audience { get; set; }
    public required string Authority { get; set; }
    public required string ClientId { get; set; }
}
