using ProjectTracker.Application.Features.Users.GetUsers;
using Riok.Mapperly.Abstractions;

namespace ProjectTracker.Application.Features.Projects.GetProject;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal partial class ProjectDtoMapper
{
    public partial ProjectDto ToDto(Project source);

    private static UsersDto MapMembers(ProjectMember member) =>
        new(member.User.Id, member.User.Name);
}
