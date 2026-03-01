using ProjectTracker.Application.Features.Common;
using ProjectTracker.Application.Features.Users.GetUsers;

namespace ProjectTracker.Application.Features.Issues.GetIssue;

public record IssueDto(
    Guid Id,
    string Key,
    string Title,
    string? Description,
    UsersDto Reporter,
    Guid ProjectId,
    UsersDto? Assignee,
    IssueStatus Status,
    IssueType Type,
    IssuePriority Priority,
    DateTimeOffset? DueDate,
    int? EstimationMinutes,
    Guid? ParentIssueId,
    Guid CreatedBy,
    DateTimeOffset CreatedAt,
    Guid UpdatedBy,
    DateTimeOffset UpdatedAt,
    IReadOnlyCollection<UsersDto> Watchers,
    IReadOnlyCollection<AttachmentDto> Attachments
);
