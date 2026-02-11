using Riok.Mapperly.Abstractions;

namespace ProjectTracker.Features.Projects.Common;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ProjectDtoMapper
{
    public partial ProjectDto ToDto(Project user);
}
