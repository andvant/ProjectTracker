namespace ProjectTracker.Application.Features.Projects.Common;

public record ProjectDto(
    Guid Id,
    string Key,
    string Name,
    string? Description,
    Guid OwnerId,
    Guid CreatedBy,
    DateTime CreatedOn,
    Guid UpdatedBy,
    DateTime UpdatedOn
);
