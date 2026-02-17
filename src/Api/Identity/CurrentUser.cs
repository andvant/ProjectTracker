using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ProjectTracker.Application.Common;

namespace ProjectTracker.Api.Identity;

internal class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId => Guid.Parse(_httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new Exception("Failed to parse UserId from claims"));
}
