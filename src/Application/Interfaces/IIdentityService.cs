namespace ProjectTracker.Application.Interfaces;

public interface IIdentityService
{
    Task AddUserToGroup(Guid userId, Guid groupId, CancellationToken ct);
    Task RemoveUserFromGroup(Guid userId, Guid groupId, CancellationToken ct);
    Task<IReadOnlyCollection<GroupDto>> GetAllGroups(CancellationToken ct);
    Task<IReadOnlyCollection<GroupDto>> GetUserGroups(Guid userId, CancellationToken ct);
}

public record GroupDto(Guid Id, string Name, string Description);
