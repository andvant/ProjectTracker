namespace ProjectTracker.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public Guid CreatedBy { get; set; }
    public DateTimeOffset CreatedOn { get; set; }
    public Guid UpdatedBy { get; set; }
    public DateTimeOffset UpdatedOn { get; set; }
}
