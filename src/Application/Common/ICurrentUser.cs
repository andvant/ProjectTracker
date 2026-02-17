namespace ProjectTracker.Application.Common;

public interface ICurrentUser
{
    Guid UserId { get; }
}
