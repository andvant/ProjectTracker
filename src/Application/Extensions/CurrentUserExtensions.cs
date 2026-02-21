namespace ProjectTracker.Application.Extensions;

internal static class CurrentUserExtensions
{
    public static void ValidateAllowed(
        this ICurrentUser currentUser,
        IReadOnlyCollection<Guid>? allowedUserIds = null,
        IReadOnlyCollection<string>? allowedRoles = null)
    {
        if (currentUser.IsAdmin()) return;

        if (allowedUserIds?.Any() == true)
        {
            var userId = currentUser.GetUserId();

            if (allowedUserIds.Contains(userId))
            {
                return;
            }
        }

        if (allowedRoles?.Any() == true)
        {
            var userRoles = currentUser.GetRoles();

            if (userRoles.Any(userRole => allowedRoles.Contains(userRole)))
            {
                return;
            }
        }

        throw new ActionForbiddenException();
    }
}
