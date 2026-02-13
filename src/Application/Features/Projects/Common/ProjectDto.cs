namespace ProjectTracker.Application.Features.Projects.Common;

public record ProjectDto(Guid Id, string Key, string Name, string? Description);
