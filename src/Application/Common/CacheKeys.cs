namespace ProjectTracker.Application.Common;

public static class CacheKeys
{
    public static string UserExists(Guid UserId) => $"UserExists:{UserId}";
    public static string UserGroups(Guid UserId) => $"UserGroups:{UserId}";
    public static string AttachmentDownload(Guid TempId) => $"AttachmentDownload:{TempId}";
}
