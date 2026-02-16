using Riok.Mapperly.Abstractions;

namespace ProjectTracker.Application.Features.Projects.GetProjects;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class ProjectsDtoMapper
{
    public static partial ProjectsDto ToDto(this Project source);

    public static partial IQueryable<ProjectsDto> ProjectToDto(this IQueryable<Project> query);
}
