namespace ProjectTracker.Application.Features.Issues.Common;

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
    DateTime UpdatedOn
);
