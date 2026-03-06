using ProjectTracker.Application.Interfaces;

namespace ProjectTracker.Infrastructure.Notifications;

internal class NotificationMessageFactory : INotificationMessageFactory
{
    public string MemberAdded(string projectKey, string projectName) =>
        $"""
        You were added as a member to the project <a href="/projects/{projectKey}">{projectName}</a>
        """;
}
