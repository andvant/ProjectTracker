using Riok.Mapperly.Abstractions;

namespace ProjectTracker.Application.Features.Users.GetUser;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
internal partial class UserDtoMapper
{
    public partial UserDto ToDto(User source);
}
