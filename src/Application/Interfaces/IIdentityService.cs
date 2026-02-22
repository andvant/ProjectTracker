namespace ProjectTracker.Application.Interfaces;

public interface IIdentityService
{
    Task AddUserToGroup(Guid userId, Guid groupId, CancellationToken ct);
    Task DeleteUserFromGroup(Guid userId, Guid groupId, CancellationToken ct);
    Task<IReadOnlyCollection<GroupDto>> GetAllGroups(CancellationToken ct);
    Task<IReadOnlyCollection<GroupDto>> GetUserGroups(Guid userId, CancellationToken ct);
}

public class GroupDto
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
}
