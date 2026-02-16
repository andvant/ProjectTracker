using ProjectTracker.Application.Features.Users.GetUsers;
using Riok.Mapperly.Abstractions;

namespace ProjectTracker.Application.Features.Issues.GetIssue;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal partial class IssueDtoMapper
{
    public partial IssueDto ToDto(Issue source);

    private static UsersDto MapWatchers(IssueWatcher watcher) =>
        new(watcher.User.Id, watcher.User.Name);
}
