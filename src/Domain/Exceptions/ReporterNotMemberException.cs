namespace ProjectTracker.Domain.Exceptions;

public class ReporterNotMemberException : DomainException
{
    public Guid ReporterId { get; }

    public ReporterNotMemberException(Guid reporterId)
        : base($"Reporter with id '{reporterId}' is not a member of the project.")
    {
        ReporterId = reporterId;
    }
}
