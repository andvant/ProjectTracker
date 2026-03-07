using ProjectTracker.Api.Endpoints.Issues;
using ProjectTracker.Api.Endpoints.Notifications;
using ProjectTracker.Api.Endpoints.Projects;
using ProjectTracker.Api.Endpoints.Users;

namespace ProjectTracker.Api.Endpoints;

internal static class EndpointNames
{
    public const string GetProject = nameof(GetProject);
    public const string GetIssue = nameof(GetIssue);
}

internal static class Endpoints
{
    public static void MapAllEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapProjects();
        app.MapIssues();
        app.MapUsers();
        app.MapNotifications();
    }

    private static void MapProjects(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/projects")
            .WithTags("Projects");

        group.MapGetProject();
        group.MapGetProjects();
        group.MapCreateProject();
        group.MapUpdateProject();
        group.MapDeleteProject();
        group.MapTransferOwnership();
        group.MapAddMember();
        group.MapRemoveMember();
        group.MapUploadProjectAttachment();
        group.MapDownloadProjectAttachment();
        group.MapRemoveProjectAttachment();
    }

    private static void MapIssues(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/projects/{projectId:guid}/issues")
            .WithTags("Issues");

        group.MapGetIssue();
        group.MapGetIssues();
        group.MapCreateIssue();
        group.MapUpdateIssue();
        group.MapDeleteIssue();
        group.MapAddWatcher();
        group.MapRemoveWatcher();
        group.MapAddComment();
        group.MapUploadIssueAttachment();
        group.MapDownloadIssueAttachment();
        group.MapRemoveIssueAttachment();
    }

    private static void MapUsers(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/users")
            .WithTags("Users");

        group.MapGetUser();
        group.MapGetUsers();
        group.MapGetUserGroups();
        group.MapUpdateUserGroups();
    }

    private static void MapNotifications(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/notifications")
            .WithTags("Notifications");

        group.MapGetNotifications();
        group.MapGetUnreadCount();
        group.MapMarkRead();
    }
}
