namespace ProjectTracker.Application.Features.Issues.GetIssues;

public record IssuesDto(
    Guid Id,
    string Key,
    string Title,
    IssueStatus Status,
    IssueType Type,
    IssuePriority Priority
);
