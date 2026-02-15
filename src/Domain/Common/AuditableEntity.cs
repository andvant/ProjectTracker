namespace ProjectTracker.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public Guid CreatedBy { get; protected set; }
    public DateTimeOffset CreatedOn { get; protected set; }
    public Guid UpdatedBy { get; protected set; }
    public DateTimeOffset UpdatedOn { get; protected set; }
}
