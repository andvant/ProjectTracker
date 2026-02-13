using Riok.Mapperly.Abstractions;

namespace ProjectTracker.Application.Features.Issues.GetIssues;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal partial class IssuesDtoMapper
{
    public partial IssuesDto ToDto(Issue source);
}
