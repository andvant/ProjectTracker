namespace ProjectTracker.Application.Common;

public static class CacheKeys
{
    public static string UserGroups(Guid UserId) => $"UserGroups:{UserId}";
    public static string UserExists(Guid UserId) => $"UserExists:{UserId}";
}
