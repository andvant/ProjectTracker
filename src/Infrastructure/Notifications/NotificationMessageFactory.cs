using ProjectTracker.Application.Interfaces;

namespace ProjectTracker.Infrastructure.Notifications;

internal class NotificationMessageFactory : INotificationMessageFactory
{
    public string MemberAdded(string projectKey, string projectName) =>
        $"""
        You were added as a member to the project <a href="{ProjectLink(projectKey)}">{projectName}</a>
        """;

    public string NewOwner(string projectKey, string projectName) =>
        $"""
        You are the new owner of the project <a href="{ProjectLink(projectKey)}">{projectName}</a>
        """;

    public string IssueAssigned(string projectKey, string issueKey, string issueTitle) =>
        $"""
        You were assigned the issue <a href="{IssueLink(projectKey, issueKey)}">{issueTitle}</a>
        """;

    public string IssueUpdated(string projectKey, string issueKey, string issueTitle) =>
        $"""
        The issue <a href="{IssueLink(projectKey, issueKey)}">{issueTitle}</a> was updated
        """;

    private string ProjectLink(string projectKey) => $"/projects/{projectKey}";
    private string IssueLink(string projectKey, string issueKey) => $"/projects/{projectKey}/issues/{issueKey}";
}
