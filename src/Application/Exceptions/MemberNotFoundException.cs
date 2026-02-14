namespace ProjectTracker.Application.Exceptions;

public class MemberNotFoundException : ApplicationException
{
    public Guid MemberId { get; }

    public MemberNotFoundException(Guid memberId)
        : base($"Member with id '{memberId}' was not found.")
    {
        MemberId = memberId;
    }
}
