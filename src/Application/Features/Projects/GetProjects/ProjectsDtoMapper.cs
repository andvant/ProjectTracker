using Riok.Mapperly.Abstractions;

namespace ProjectTracker.Application.Features.Projects.GetProjects;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal partial class ProjectsDtoMapper
{
    public partial ProjectsDto ToDto(Project source);
}
