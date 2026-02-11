using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Issues.Common;

public record IssueDto(
    Guid Id,
    string ShortName,
    string Title,
    string? Description,
    Guid CreatorId,
    Guid ProjectId,
    DateTime Created,
    DateTime Updated,
    Guid? AssigneeId,
    IssueStatus Status,
    IssuePriority Priority);
