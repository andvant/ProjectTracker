using Riok.Mapperly.Abstractions;

namespace ProjectTracker.Application.Features.Projects.GetProject;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal partial class ProjectDtoMapper
{
    public partial ProjectDto ToDto(Project source);
}
