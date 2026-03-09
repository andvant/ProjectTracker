namespace ProjectTracker.Application.Interfaces;

public interface ICurrentUser
{
    Guid GetUserId();
    IReadOnlyCollection<string> GetRoles();
    bool IsAdmin();
    bool IsProjectManager();
}
