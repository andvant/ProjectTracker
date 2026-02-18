using ProjectTracker.Application.Features.Common;
using ProjectTracker.Application.Features.Users.GetUsers;
using Riok.Mapperly.Abstractions;

namespace ProjectTracker.Application.Features.Projects.GetProject;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class ProjectDtoMapper
{
    public static partial ProjectDto ToDto(this Project source);

    public static partial IQueryable<ProjectDto> ProjectToDto(this IQueryable<Project> query);

    private static UsersDto MapMembers(ProjectMember member) =>
        new(member.User.Id, member.User.Name);

    private static AttachmentDto MapAttachments(ProjectAttachment attachment) =>
        new(attachment.Attachment.Id, attachment.Attachment.Name);
}
