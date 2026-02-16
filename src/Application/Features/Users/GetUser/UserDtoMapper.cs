using ProjectTracker.Application.Features.Projects.GetProjects;
using Riok.Mapperly.Abstractions;

namespace ProjectTracker.Application.Features.Users.GetUser;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class UserDtoMapper
{
    public static partial UserDto ToDto(this User source);

    public static partial IQueryable<UserDto> ProjectToDto(this IQueryable<User> query);

    private static ProjectsDto MapProjects(ProjectMember member) =>
        new(member.Project.Id, member.Project.Key, member.Project.Name);
}
