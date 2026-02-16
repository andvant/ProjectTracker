using Riok.Mapperly.Abstractions;

namespace ProjectTracker.Application.Features.Issues.GetIssues;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class IssuesDtoMapper
{
    public static partial IssuesDto ToDto(this Issue source);

    public static partial IQueryable<IssuesDto> ProjectToDto(this IQueryable<Issue> query);
}
