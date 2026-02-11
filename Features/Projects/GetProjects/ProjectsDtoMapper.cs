using Riok.Mapperly.Abstractions;

namespace ProjectTracker.Features.Projects.GetProjects;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class ProjectsDtoMapper
{
    public partial ProjectsDto ToDto(Project user);
}
