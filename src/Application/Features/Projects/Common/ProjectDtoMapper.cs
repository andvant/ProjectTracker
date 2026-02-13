using Riok.Mapperly.Abstractions;

namespace ProjectTracker.Application.Features.Projects.Common;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal partial class ProjectDtoMapper
{
    public partial ProjectDto ToDto(Project source);
}
