using Riok.Mapperly.Abstractions;

namespace ProjectTracker.Application.Features.Users.GetUsers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal static partial class UsersDtoMapper
{
    public static partial UsersDto ToDto(this User source);

    public static partial IQueryable<UsersDto> ProjectToDto(this IQueryable<User> query);
}
