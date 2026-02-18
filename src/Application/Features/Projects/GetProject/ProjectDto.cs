using ProjectTracker.Application.Features.Common;
using ProjectTracker.Application.Features.Issues.GetIssues;
using ProjectTracker.Application.Features.Users.GetUsers;

namespace ProjectTracker.Application.Features.Projects.GetProject;

public record ProjectDto(
    Guid Id,
    string Key,
    string Name,
    string? Description,
    Guid OwnerId,
    Guid CreatedBy,
    DateTimeOffset CreatedOn,
    Guid UpdatedBy,
    DateTimeOffset UpdatedOn,
    IReadOnlyCollection<UsersDto> Members,
    IReadOnlyCollection<IssuesDto> Issues,
    IReadOnlyCollection<AttachmentDto> Attachments
);
