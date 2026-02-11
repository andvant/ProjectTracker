using Riok.Mapperly.Abstractions;

namespace ProjectTracker.Application.Features.Issues.Common;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class IssueDtoMapper
{
    public partial IssueDto ToDto(Issue source);
}
