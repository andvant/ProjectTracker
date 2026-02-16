using ProjectTracker.Application.Features.Issues.GetIssues;
using ProjectTracker.Application.Features.Projects.GetProjects;

namespace ProjectTracker.Application.Features.Users.GetUser;

public record UserDto(
    Guid Id,
    string Name,
    string Email,
    DateTimeOffset RegistrationDate,
    IReadOnlyCollection<ProjectsDto> Projects,
    IReadOnlyCollection<IssuesDto> AssignedIssues
);
