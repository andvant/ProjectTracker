using Riok.Mapperly.Abstractions;

namespace ProjectTracker.Application.Features.Issues.GetIssues;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class IssuesDtoMapper
{
    public partial IssuesDto ToDto(Issue source);
}
