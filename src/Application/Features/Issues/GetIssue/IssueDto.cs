using ProjectTracker.Application.Features.Users.GetUsers;

namespace ProjectTracker.Application.Features.Issues.GetIssue;

public record IssueDto(
    Guid Id,
    string Key,
    string Title,
    string? Description,
    Guid ReporterId,
    Guid ProjectId,
    Guid? AssigneeId,
    IssueStatus Status,
    IssueType Type,
    IssuePriority Priority,
    DateTime? DueDate,
    int? EstimationMinutes,
    Guid? ParentIssueId,
    Guid CreatedBy,
    DateTime CreatedOn,
    Guid UpdatedBy,
    DateTime UpdatedOn,
    IReadOnlyCollection<UsersDto> Watchers
);
