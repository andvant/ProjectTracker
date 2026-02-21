using System.Security.Claims;
using ProjectTracker.Application.Common;
using ProjectTracker.Application.Interfaces;

namespace ProjectTracker.Api.Identity;

internal class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid GetUserId() => Guid.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new Exception("Failed to parse UserId from claims."));

    public IReadOnlyCollection<string> GetRoles() => _httpContextAccessor.HttpContext?.User?
        .FindAll(ClaimTypes.Role).Select(c => c.Value).ToList() ?? [];

    public bool IsAdmin() => GetRoles()?.Any(r => r == Roles.Admin) == true;
    public bool IsProjectManager() => GetRoles()?.Any(r => r == Roles.ProjectManager) == true;
}
