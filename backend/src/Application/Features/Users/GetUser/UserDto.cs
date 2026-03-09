namespace ProjectTracker.Application.Features.Users.GetUser;

public record UserDto(
    Guid Id,
    string Username,
    string Email,
    string FullName,
    DateTimeOffset RegistrationDate
);
