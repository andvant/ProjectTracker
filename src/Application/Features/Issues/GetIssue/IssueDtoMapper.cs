using ProjectTracker.Application.Features.Common;
using ProjectTracker.Application.Features.Users.GetUsers;
using Riok.Mapperly.Abstractions;

namespace ProjectTracker.Application.Features.Issues.GetIssue;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class IssueDtoMapper
{
    public static partial IssueDto ToDto(this Issue source);

    public static partial IQueryable<IssueDto> ProjectToDto(this IQueryable<Issue> query);

    private static UsersDto MapWatchers(IssueWatcher watcher) =>
        new(watcher.User.Id, watcher.User.Name);

    private static AttachmentDto MapAttachments(IssueAttachment attachment) =>
        new(attachment.Attachment.Id, attachment.Attachment.Name);
}
