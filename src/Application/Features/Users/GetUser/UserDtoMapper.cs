using Riok.Mapperly.Abstractions;

namespace ProjectTracker.Application.Features.Users.GetUser;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class UserDtoMapper
{
    public static partial UserDto ToDto(User source);

    public static partial IQueryable<UserDto> ProjectToDto(this IQueryable<User> query);
}
