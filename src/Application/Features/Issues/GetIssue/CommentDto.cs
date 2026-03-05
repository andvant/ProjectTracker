using ProjectTracker.Application.Features.Users.GetUsers;

namespace ProjectTracker.Application.Features.Issues.GetIssue;

public record CommentDto(Guid Id, UsersDto User, string Text);
