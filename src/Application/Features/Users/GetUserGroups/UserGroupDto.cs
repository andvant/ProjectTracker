namespace ProjectTracker.Application.Features.Users.GetUserGroups;

public record UserGroupDto(
    Guid Id,
    string Name,
    string Description,
    bool IsMember
);
