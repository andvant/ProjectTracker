namespace ProjectTracker.Application.Interfaces;

public interface INotificationMessageFactory
{
    string MemberAdded(string projectKey, string projectName);
}
