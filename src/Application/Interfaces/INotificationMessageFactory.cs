namespace ProjectTracker.Application.Interfaces;

public interface INotificationMessageFactory
{
    string MemberAdded(string projectKey, string projectName);
    string NewOwner(string projectKey, string projectName);
    string IssueAssigned(string projectKey, string issueKey, string issueTitle);
    string IssueUpdated(string projectKey, string issueKey, string issueTitle);
}
