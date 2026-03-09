using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using ProjectTracker.Application.Interfaces;

namespace ProjectTracker.Infrastructure.Identity;

public class KeycloakService : IIdentityService
{
    private readonly HttpClient _httpClient;
    private readonly KeycloakAdminConfig _config;

    public KeycloakService(HttpClient httpClient, IOptions<KeycloakAdminConfig> config)
    {
        _httpClient = httpClient;
        _config = config.Value;
    }

    public async Task<IReadOnlyCollection<GroupDto>> GetAllGroups(CancellationToken ct)
    {
        await SetAccessToken(ct);

        var url = $"admin/realms/{_config.Realm}/groups";

        var response = await _httpClient.GetAsync(url, ct);

        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<IReadOnlyCollection<GroupDto>>(ct))!;
    }

    public async Task<IReadOnlyCollection<GroupDto>> GetUserGroups(Guid userId, CancellationToken ct)
    {
        await SetAccessToken(ct);

        var url = $"admin/realms/{_config.Realm}/users/{userId}/groups";

        var response = await _httpClient.GetAsync(url, ct);

        response.EnsureSuccessStatusCode();

        return (await response.Content.ReadFromJsonAsync<IReadOnlyCollection<GroupDto>>(ct))!;
    }

    public async Task RemoveUserFromGroup(Guid userId, Guid groupId, CancellationToken ct)
    {
        await SetAccessToken(ct);

        var url = $"admin/realms/{_config.Realm}/users/{userId}/groups/{groupId}";

        var response = await _httpClient.DeleteAsync(url, ct);

        response.EnsureSuccessStatusCode();
    }

    public async Task AddUserToGroup(Guid userId, Guid groupId, CancellationToken ct)
    {
        await SetAccessToken(ct);

        var url = $"admin/realms/{_config.Realm}/users/{userId}/groups/{groupId}";

        var response = await _httpClient.PutAsync(url, null, ct);

        response.EnsureSuccessStatusCode();
    }

    private async Task SetAccessToken(CancellationToken ct)
    {
        if (_httpClient.DefaultRequestHeaders.Authorization is null)
        {
            var accessToken = await GetAccessToken(ct);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
    }

    private async Task<string> GetAccessToken(CancellationToken ct)
    {
        var url = $"realms/{_config.Realm}/protocol/openid-connect/token";

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["grant_type"] = "client_credentials",
            ["client_id"] = _config.ClientId,
            ["client_secret"] = _config.ClientSecret
        });

        var response = await _httpClient.PostAsync(url, content, ct);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadFromJsonAsync<KeycloakTokenResponse>(ct);

        return json!.AccessToken;
    }

    internal class KeycloakTokenResponse
    {
        [JsonPropertyName("access_token")]
        public required string AccessToken { get; set; }
    }
}
