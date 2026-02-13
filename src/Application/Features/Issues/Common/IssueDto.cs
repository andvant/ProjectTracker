using ProjectTracker.Domain.Enums;

namespace ProjectTracker.Application.Features.Issues.Common;

public record IssueDto(
    Guid Id,
    string ShortName,
    string Title,
    string? Description,
    Guid ReporterId,
    Guid ProjectId,
    DateTime CreatedOn,
    DateTime UpdatedOn,
    Guid? AssigneeId,
    IssueStatus Status,
    IssuePriority Priority);
