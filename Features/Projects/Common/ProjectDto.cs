namespace ProjectTracker.Features.Projects.Common;

public record ProjectDto(Guid Id, string ShortName, string Name, string? Description);
