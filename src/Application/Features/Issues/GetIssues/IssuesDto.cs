using ProjectTracker.Application.Features.Users.GetUsers;

namespace ProjectTracker.Application.Features.Issues.GetIssues;

public record IssuesDto(
    Guid Id,
    string Key,
    string Title,
    UsersDto? Assignee,
    IssueStatus Status,
    IssueType Type,
    IssuePriority Priority,
    DateTimeOffset UpdatedAt
);
