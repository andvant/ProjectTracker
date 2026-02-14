using Riok.Mapperly.Abstractions;

namespace ProjectTracker.Application.Features.Users.GetUsers;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal partial class UsersDtoMapper
{
    public partial UsersDto ToDto(User source);
}
