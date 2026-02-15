namespace ProjectTracker.Application.Features.Users.GetUser;

public record UserDto(
    Guid Id,
    string Name,
    string Email,
    DateTimeOffset RegistrationDate
);
